namespace CelestialBodies
{ 
    public interface ICelestialObserver
    {
        public void OnShapeUpdate();


        // TODO IMPLEMENT
        public void OnShadingUpdate();
        public void OnInitialUpdate();
    }
}