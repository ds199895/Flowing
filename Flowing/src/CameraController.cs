using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    public class CameraController
    {
        private GameWindow window;
        private IApp app;
        private Camera camPerspective;
        public CameraController(IApp app,double dist)
        {
            this.app = app;
            this.window =this.app.window;
            ResetCamera(dist);
            app.window.UpdateFrame += this.Window_UpdateFrame;

        }

        public void ResetCamera(double dist)
        {
            double w = (double)this.app.width;
            double h = (double)this.app.height;
            this.camPerspective = new Camera(w, h, new Vector3((float)(app.width/2), (float)(app.height / 2), (float)dist), new Vector3((float)(app.width / 2), (float)(app.height / 2), 0),new Vector3(0,1,0));
        }

        private void Window_UpdateFrame(Object sender, EventArgs e)
        {
            this.camPerspective.applyCamera(this.app);
            this.window.SwapBuffers();
        }
    }
}
