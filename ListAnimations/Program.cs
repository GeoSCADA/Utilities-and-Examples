using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewX;

namespace ListAnimations
{
	class Program
	{
		static void Main(string[] args)
		{
			string SystemName = args[0];
			string UserName = args[1];
			string Password = args[2];
			string MimicName = "";
			if (args.Count() == 4)
			{
				MimicName = args[3];
			}

			ViewX.Application vwx = new ViewX.Application();
			vwx.Logon(SystemName, UserName, Password);
			Console.WriteLine("Logged in to ViewX");

			ScxV6DbClient.ScxV6Server db = new ScxV6DbClient.ScxV6Server();
			db.Connect(SystemName, UserName, Password);
			Console.WriteLine("Logged in to Server");

			// Hide ViewX for performance
			vwx.Visible = false;
			if (MimicName == "")
			{
				var root = db.RootObject;
				Console.WriteLine("Got root object");
				FindAllMimics(root, vwx, SystemName, db);
			}
			else
			{
				var mimic = db.FindObject(MimicName);
				ProcessMimic(vwx, SystemName, mimic, db);
			}
			vwx.Visible = true;
			Console.ReadKey();
		}

		static void FindAllMimics( ScxV6DbClient.IScxV6Object group, ViewX.Application vwx, String SystemName, ScxV6DbClient.ScxV6Server db)
		{
			var groups = group.List("CGroup");
			foreach( ScxV6DbClient.IScxV6Object childgroup in groups)
			{
				Console.WriteLine("Group, " + childgroup.FullName.Replace("\"", "\"\""));
				FindAllMimics(childgroup, vwx, SystemName, db);
			}
			var mimics = group.List("CMimic");
			foreach (ScxV6DbClient.IScxV6Object mimic in mimics)
			{
				ProcessMimic(vwx, SystemName, mimic, db);
			}
		}

		private static void ProcessMimic(Application vwx, string SystemName, ScxV6DbClient.IScxV6Object mimic, ScxV6DbClient.ScxV6Server db)
		{
			Console.WriteLine("Mimic, " + mimic.FullName.Replace("\"", "\"\""));
			try
			{

				// Open in ViewX
				ViewX.Mimic mim = (Mimic)vwx.Mimics.OpenFromServer(true, SystemName, mimic.FullName);

				// List animations
				foreach (ViewX.DrwGroup Layer in mim.Layers)
				{
					mim.ActiveLayer = Layer;
					foreach (ViewX.DrwObject Item in Layer)
					{
						if (Item.Type == DrwType.DrwEmbeddedList)
						{
							ViewX.DrwEmbeddedList List = (ViewX.DrwEmbeddedList)Item;
							Console.WriteLine("\"" + mim.FullName.Replace("\"", "\"\"") + "\",\"" + Layer.Name.Replace("\"", "\"\"") + "\",\"" + Item.Name.Replace("\"", "\"\"") + "\",\"" + List.Sql.Replace("\"", "\"\"") + "\"");
						}
						foreach (ViewX.DrwAnimation Ani in Item.Animations)
						{
							Console.WriteLine("\"" + mim.FullName.Replace("\"", "\"\"") + "\",\"" + Layer.Name.Replace("\"", "\"\"") + "\",\"" + Item.Name.Replace("\"", "\"\"") + "\",\"" + Ani.Property.Replace("\"", "\"\"") + "\",\"" + Ani.Expression.Replace("\"", "\"\"") + "\"");
						}
						// Process embedded objects
						if (Item.Type == DrwType.DrwEmbeddedMimic)
						{
							ViewX.DrwEmbeddedMimic mimic1 = (ViewX.DrwEmbeddedMimic)Item;
							Console.WriteLine("\"" + mim.FullName.Replace("\"", "\"\"") + "\",Embeds,\"" + Layer.Name.Replace("\"", "\"\"") + "\",\"" + Item.Name.Replace("\"", "\"\"") + "\",\"" + mimic1.Shared + "\",\"" + mimic1.Definition.Replace("\"", "\"\"") + "\"");
							string MimicName = mimic1.Definition; // Starts SCX:////CMimic/
							int NameStart = MimicName.IndexOf("/CMimic/") + 8;
							MimicName = MimicName.Substring(NameStart);
							if (MimicName.StartsWith("."))
							{
								//'need to convert relative to absolute, using path of mimicName
								//'get # of dots to left of mName e.g. ..A.B -> ,,A,B
								//'e.g. if .A then i=1, if ..A.B then i = 2
								string[] mNameArr = MimicName.Split('.');
								int j = 0;
								for (int i = 0; i < mNameArr.Count(); i++)
								{
									if (mNameArr[i] != "")
									{
										j = i;
										break;
									}
								}
								//'now split parent mimic path, e.g. a.b.c -> a,b,c
								string[] mParentPath = mimic.FullName.Split('.');
								MimicName = "";
								//'Path folder names but not mimic name (-1), remove 1 more for each .
								for (int i = 0; i < mParentPath.Count() - j - 1; i++) 
								{
									MimicName = MimicName + mParentPath[i] + ".";
								}
								//'now append non-blank names from relative path
								for (int i = 0; i < mNameArr.Count(); i++)
								{
									if (mNameArr[i] != "")
									{
										MimicName = MimicName + mNameArr[i] + ".";
									}
								}
								//'remove extra .
								MimicName = MimicName.Substring(0, MimicName.Length - 1);
							}

							var embeddedmimic = db.FindObject(MimicName);
							if (embeddedmimic != null)
							{
								ProcessMimic(vwx, SystemName, embeddedmimic, db);
							}
							else
							{
								Console.WriteLine("Error, cannot open embedded" );
							}
						}
					}
				}

				// Close
				mim.Close();
				Console.WriteLine();
			}
			catch (Exception e)
			{
				Console.WriteLine("Error, " + e.Message.Replace("\"", "\"\""));
			}
		}
	}
}
