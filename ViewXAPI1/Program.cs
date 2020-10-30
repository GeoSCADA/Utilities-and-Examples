using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewX;

namespace ViewXAPI1
{
	class Program
	{
		static void Main(string[] args)
		{
			ViewX.Application a = new ViewX.Application();
			a.Logon("Local", "s", "s");
			a.Mimics.OpenFromServer(false, "Local", "");
			a.Visible = true;
			ViewX.Mimic oMimic = (Mimic) a.Mimics.OpenFromServer(false, "Local", "New Mimic");
			ViewX.DrwLayers oLayers = oMimic.Layers;
			oLayers.Add("Extra");
			var oSquareRef = oLayers["Extra"].AddRectangle(100000, 150000, 500000, 650000);
			oLayers["Extra"].SelectObject( (DrwObject) oSquareRef);
			oMimic.ActiveLayer = oLayers["Extra"];
			oMimic.DeleteSelected();
			oMimic.Save();
			oMimic.Close();

		}
	}
}
