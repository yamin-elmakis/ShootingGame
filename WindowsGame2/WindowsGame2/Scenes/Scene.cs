using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game.Scenes
{
    public abstract class Scene:DrawableGameComponent
    {
        public List<GameComponent> SceneComponents { get; set; }
        public abstract int State { get; set; }

        protected Scene(Microsoft.Xna.Framework.Game game) : base(game){
            SceneComponents = new List<GameComponent>();
            Visible = false;
            Enabled = false;
        }
        public void Show(){
            Enabled = true;
            Visible = true;
        }
        public void Hide(){
            Enabled = false;
            Visible = false;
        }

        public void Pause(){
            Enabled = false;
            Visible = true;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in SceneComponents.Where(component => component.Enabled))
            {
                component.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var component in SceneComponents.OfType<DrawableGameComponent>().Where(component => component.Visible))
            {
                component.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
    }
}
