namespace AwaraIT.BCS.Application.Core
{
    public struct Void
    {
        public static implicit operator Void(string value)
        {
            return new Void();
        }
    }
}
