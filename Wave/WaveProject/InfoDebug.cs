using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace WaveProject
{
    public class InfoDebug : Behavior
    {
        public TextBlock Text { get; private set; }

        public InfoDebug(TextBlock text) : base()
        {
            //Create UI
            Text = text;
            Text.Foreground = Color.White;
        }

        protected override void Update(TimeSpan gameTime)
        {
            Text.Text = string.Format("Coords. ({0},{1}), dt: {2}", WaveServices.Input.MouseState.X, WaveServices.Input.MouseState.Y, (float)gameTime.TotalSeconds);
        }
    }
}
