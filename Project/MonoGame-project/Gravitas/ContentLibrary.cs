using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravitas
{
    /// <summary>
    /// Author: Jackson Luff
    /// </summary>
    static public class ContentLibrary
    {
        //Splash textures
        static public Texture2D teamLogoTexture;

        //In-game textures
        static public Texture2D circleTexture;
        static public Texture2D turretBaseTexture;
        static public Texture2D turretTopTexture;
        static public Texture2D platformTexture;

        //Fonts
        static public SpriteFont arialFont; 
        static public SpriteFont consolasFont;
        static public SpriteFont timesNewRomanFont;

        //Shaders / Effects
        static public Effect bloomExtractEffect;
        static public Effect bloomCombineEffect;
        static public Effect gaussianBlurEffect;

        /// <summary>
        /// This method creates and applies content to all static variables
        /// available for use everywhere that ContentLibrary is mentioned
        /// </summary>
        /// <param name="a_cManage">Reference to Content Manager inside Game1 : Game</param> 
        public static void LoadContentLibrary(ContentManager a_cManage)
        {
            //Splash textures
            teamLogoTexture    = a_cManage.Load<Texture2D>("teamLogo");

            //In-game textures
            circleTexture      = a_cManage.Load<Texture2D>("Circle");
            turretBaseTexture  = a_cManage.Load<Texture2D>("TurretBase");
            turretTopTexture   = a_cManage.Load<Texture2D>("TurretTop");
            platformTexture    = a_cManage.Load<Texture2D>("Platform");

            //Fonts
            arialFont          = a_cManage.Load<SpriteFont>("Fonts/Arial");
            consolasFont       = a_cManage.Load<SpriteFont>("Fonts/Consolas");
            timesNewRomanFont  = a_cManage.Load <SpriteFont>("Fonts/TimesNewRoman");

#if !PSM
            //Shaders / Effects
            bloomExtractEffect = a_cManage.Load<Effect>("Shaders/BloomExtract");
            bloomCombineEffect = a_cManage.Load<Effect>("Shaders/BloomCombine");
            gaussianBlurEffect = a_cManage.Load<Effect>("Shaders/GaussianBlur");
#endif
        }

    }
}
