using RCT2ObjectData.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects.Types.AttractionInfo {
    /**<summary>A car frame with x swinging frames and y animation frames.</summary>*/
    public class CarFrame {
	    //=========== MEMBERS ============
	    #region Members

	    /**<summary>The image entry info for each frames.</summary>*/
	    public ImageEntry[,] Entries;
	    /**<summary>The image for each frames.</summary>*/
	    public PaletteImage[,] Images;

	    #endregion
	    //========= CONSTRUCTORS =========
	    #region Constructors

	    /**<summary>Constructs a car frame with the specified frames.</summary>*/
	    public CarFrame(int swingingFrames, int animationFrames) {
		    Entries		= new ImageEntry[swingingFrames, animationFrames];
		    Images		= new PaletteImage[swingingFrames, animationFrames];
	    }

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets or sets the single image entry.</summary>*/
		public ImageEntry Entry {
		    get { return Entries[0, 0]; }
		    set { Entries[0, 0] = value; }
	    }
	    /**<summary>Gets or sets the single image.</summary>*/
	    public PaletteImage Image {
		    get { return Images[0, 0]; }
		    set { Images[0, 0] = value; }
	    }

	    #endregion
    }
}
