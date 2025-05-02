namespace Tests
{
    public static class Asserts
    {
        public static void IsType(Type type, object data)
        {
            Assert.IsTrue(type == data.GetType());
        }
    }
}
