namespace CelestialBodies
{ 
    public interface ICelestialObserver
    {
        public void OnShapeUpdate();

        public void OnShadingUpdate();
        public void OnInitialUpdate();

        public void OnPhysicsUpdate();
    }
}