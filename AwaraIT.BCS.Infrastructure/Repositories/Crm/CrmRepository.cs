using AwaraIT.BCS.Domain;
using AwaraIT.BCS.Domain.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public abstract class CrmRepository<T> where T : Entity
    {
        private const int BlockSizeDownloadDefault = 4194304;

        public IOrganizationService Service { get; }

        protected abstract string EntityName { get; }

        public CrmRepository(IOrganizationService service)
        {
            Service = service;
        }

        public Guid Create(T entity) => Service.Create(entity.ToEntity<Entity>());

        public void Update(T entity)
        {
            if (entity.Attributes.Count > 0)
            {
                Service.Update(entity.ToEntity<Entity>());
            }
        }

        public void Delete(Guid id) => Service.Delete(EntityName, id);

        public T Get(Guid id, bool allColumns) => Service.Retrieve(EntityName, id, new ColumnSet(allColumns)).ToEntity<T>();

        public T Get(Guid id, params string[] attributes) => Service.Retrieve(EntityName, id, new ColumnSet(attributes)).ToEntity<T>();

        public List<T> Get(IEnumerable<ConditionExpression> conditions, int? topCount = null, params string[] columns)
        {
            var query = new QueryExpression(EntityName)
            {
                NoLock = true,
                TopCount = topCount,
                ColumnSet = columns != null ? new ColumnSet(columns) : new ColumnSet(true)
            };

            if (conditions.HasItems())
            {
                query.Criteria = new FilterExpression();
                foreach (var condition in conditions)
                {
                    query.Criteria.AddCondition(condition);
                }
            }

            return Service.RetrieveMultiple(query).Entities.Select(e => e.ToEntity<T>()).ToList();
        }

        public List<T> Get(QueryExpression query) => Service.RetrieveMultiple(query).Entities.Select(e => e.ToEntity<T>()).ToList();

        public List<T> GetAllPaged(QueryExpression query, int pageSize = Constants.MaxPageSize)
        {
            var result = new List<Entity>();
            query.PageInfo = new PagingInfo
            {
                Count = pageSize,
                PageNumber = 1,
                PagingCookie = null
            };

            while (true)
            {
                var results = Service.RetrieveMultiple(query);
                result.AddRange(results.Entities);

                if (results.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = results.PagingCookie;
                }
                else
                {
                    break;
                }
            }

            return result.Select(e => e.ToEntity<T>()).ToList();
        }

        public OrganizationResponse Execute(OrganizationRequest request) => Service.Execute(request);

        public byte[] DownloadFile(EntityReference entityReference, string attributeName)
        {
            var initializeFileBlocksDownloadRequest = new InitializeFileBlocksDownloadRequest
            {
                Target = entityReference,
                FileAttributeName = attributeName
            };

            var initializeFileBlocksDownloadResponse = (InitializeFileBlocksDownloadResponse)Execute(initializeFileBlocksDownloadRequest);
            var fileSizeInBytes = initializeFileBlocksDownloadResponse.FileSizeInBytes;
            var fileBytes = new List<byte>((int)fileSizeInBytes);

            var offset = 0L;
            var blockSizeDownload = !initializeFileBlocksDownloadResponse.IsChunkingSupported ? fileSizeInBytes : BlockSizeDownloadDefault;

            if (fileSizeInBytes < blockSizeDownload)
                blockSizeDownload = fileSizeInBytes;

            while (fileSizeInBytes > 0)
            {
                var downLoadBlockRequest = new DownloadBlockRequest
                {
                    BlockLength = blockSizeDownload,
                    FileContinuationToken = initializeFileBlocksDownloadResponse.FileContinuationToken,
                    Offset = offset
                };

                var downloadBlockResponse = (DownloadBlockResponse)Execute(downLoadBlockRequest);

                fileBytes.AddRange(downloadBlockResponse.Data);
                fileSizeInBytes -= (int)blockSizeDownload;
                offset += blockSizeDownload;
            }

            return fileBytes.ToArray();
        }

        public void UploadFile(EntityReference entityReference, string fileAttributeName, string fileName, string mimeType, byte[] fileBody)
        {
            var initRequest = new InitializeFileBlocksUploadRequest
            {
                FileAttributeName = fileAttributeName,
                FileName = fileName,
                Target = entityReference
            };
            var initResponse = (InitializeFileBlocksUploadResponse)Execute(initRequest);

            var fileContinuationToken = initResponse.FileContinuationToken;
            var blockIds = new List<string>();

            for (int i = 0; i < Math.Ceiling(fileBody.Length / Convert.ToDecimal(BlockSizeDownloadDefault)); i++)
            {
                var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                blockIds.Add(blockId);
                var blockData = fileBody.Skip(i * BlockSizeDownloadDefault).Take(BlockSizeDownloadDefault).ToArray();
                var blockRequest = new UploadBlockRequest { FileContinuationToken = fileContinuationToken, BlockId = blockId, BlockData = blockData };
                Execute(blockRequest);
            }

            var commitRequest = new CommitFileBlocksUploadRequest
            {
                BlockList = blockIds.ToArray(),
                FileContinuationToken = fileContinuationToken,
                FileName = fileName,
                MimeType = mimeType
            };

            Execute(commitRequest);
        }
    }
}
