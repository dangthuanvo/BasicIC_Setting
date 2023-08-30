namespace BasicIC_Setting.Services.Prototye
{
    public class PrototypeFactory
    {
        private readonly IPrototype _prototypeObject;

        public PrototypeFactory(IPrototype prototypeObject)
        {
            _prototypeObject = prototypeObject;
        }

        public IPrototype GetClone(string cloneType)
        {
            if (cloneType.Equals("deep"))
                return _prototypeObject.DeepClone();
            else
                return _prototypeObject.Clone();
        }
    }
}