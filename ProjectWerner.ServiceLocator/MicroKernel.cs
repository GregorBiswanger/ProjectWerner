using Ninject;

namespace ProjectWerner.ServiceLocator
{
    public static class MicroKernel
    {
        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get
            {
                if (_kernel != null)
                {
                    return _kernel;
                }

                _kernel = new StandardKernel();

                return _kernel;
            }
        }

        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}
