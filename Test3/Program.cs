using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Collections.ObjectModel;
using Libraria;
using Libraria.Serialization;
using Libraria.Serialization.Formats;

namespace Test3 {
	class Program {
		static void Main(string[] args) {
			Console.Title = "Test3";

			if (args.Length == 0)
				args = new string[] { "q3dm0.pk3" };

			for (int i = 0; i < args.Length; i++) {
				string FileNameExt = args[i];
				string FileName = Path.GetFileNameWithoutExtension(FileNameExt);
				string Extension = Path.GetExtension(FileNameExt);
				string OutName = FileName + "_quakelive" + Extension;

				if (Extension == ".bsp") {
					Console.WriteLine("Opening " + FileNameExt);
					BSP Map = BSP.FromFile(FileNameExt);

					if (Map.Version == 46)
						Map.Version = 47;

					Console.WriteLine("Writing " + OutName);
					File.WriteAllBytes(OutName, Map.ToByteArray());
				} else if (Extension == ".pk3") {
					using (FileStream FS = File.OpenRead(FileNameExt)) {
						using (ZipArchive Origin = new ZipArchive(FS, ZipArchiveMode.Read)) {
							using (FileStream FS2 = File.Create(OutName)) {
								using (ZipArchive Dest = new ZipArchive(FS2, ZipArchiveMode.Create)) {

									foreach (var Entry in Origin.Entries) {
										string EntryExt = Path.GetExtension(Entry.FullName);
										ZipArchiveEntry OutEntry = Dest.CreateEntry(Entry.FullName, CompressionLevel.Optimal);

										ReadEntry(Entry, (InStream) => {
											WriteEntry(OutEntry, (OutStream) => {
												if (EntryExt == ".bsp") {
													Console.WriteLine("Converting {0}", Entry.FullName);

													BSP Map = BSP.FromStream(InStream);
													if (Map.Version == 46)
														Map.Version = 47;
													Map.Serialize(OutStream);
												} else
													InStream.CopyTo(OutStream);
											});
										});
									}
								}
							}
						}
					}

				} else {
					Console.WriteLine("Skipping {0}, unknown extension type {1}", FileNameExt, Extension);
				}
			}
		}

		static void ReadEntry(ZipArchiveEntry E, Action<Stream> A) {
			using (Stream S = E.Open()) {
				using (MemoryStream MS = new MemoryStream()) {
					S.CopyTo(MS);
					MS.Seek(0, SeekOrigin.Begin);
					A(MS);
				}
			}
		}

		static void WriteEntry(ZipArchiveEntry E, Action<Stream> A) {
			using (MemoryStream MS = new MemoryStream()) {
				A(MS);
				//MS.Seek(0, SeekOrigin.Begin);
				using (Stream S = E.Open()) {
					MS.CopyTo(S);
				}
			}
		}
	}
}
