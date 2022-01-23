using PaintDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SourceEngineRAW
{
    public sealed class SourceEngineRAWFactory : IFileTypeFactory
    {
        public FileType[] GetFileTypeInstances()
        {
            return new[] { new SourceEngineRAWPlugin() };
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo))]
    internal class SourceEngineRAWPlugin : FileType
    {
        /// <summary>
        /// Constructs a ExamplePropertyBasedFileType instance
        /// </summary>
        internal SourceEngineRAWPlugin()
            : base(
                "SourceEngineRAW",
                new FileTypeOptions
                {
                    LoadExtensions = new string[] { ".raw" },
                    SaveExtensions = new string[] { ".raw" },
                    SupportsCancellation = true,
                    SupportsLayers = false
                })
        {
        }

        protected override SaveConfigToken OnCreateDefaultSaveConfigToken()
        {
            return new SourceEngineRAWSaveConfigToken();
        }

        public override SaveConfigWidget CreateSaveConfigWidget()
        {
            return new SourceEngineRAWSaveConfigWidget();
        }

        /// <summary>
        /// Saves a document to a stream respecting the properties
        /// </summary>

        protected override void OnSave(Document input, Stream output, SaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)
        {
            SourceEngineRAWSaveConfigToken configToken = (SourceEngineRAWSaveConfigToken)token;

            // Render a flattened view of the Document to the scratch surface.
            input.Flatten(scratchSurface);

            // The stream paint.net hands us must not be closed.
            using (BinaryWriter writer = new BinaryWriter(output, Encoding.UTF8, leaveOpen: true))
            {
                int width;
                int height;

                if (configToken.AllowAnyResolution == false)
                {
                    width = 1024; // scratchSurface.Width
                    height = 32; // scratchSurface.Height
                }
                else
                {
                    width = scratchSurface.Width;
                    height = scratchSurface.Height;
                }

                for (int y = 0; y < height; y++)
                {
                    // Report progress if the callback is not null.
                    if (progressCallback != null)
                    {
                        double percent = (double)y / height;

                        progressCallback(null, new ProgressEventArgs(percent));
                    }

                    for (int x = 0; x < width; x++)
                    {
                        // Write the pixel values.
                        ColorBgra color = scratchSurface[x, y];

                        writer.Write(color.R);
                        writer.Write(color.G);
                        writer.Write(color.B);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a document from a stream
        /// </summary>
        protected override Document OnLoad(Stream input)
        {
            Document doc = null;

            // The stream paint.net hands us must not be closed.
            using (BinaryReader reader = new BinaryReader(input, Encoding.UTF8, leaveOpen: true))
            {
                int res = (int)Math.Pow( (input.Length / 3.0f), 1.0f / 3.0f );
                int resx3 = res * res * res;

                int height = res;
                int width = res * res;

                // Create a new Document.
                doc = new Document(width, height);
                 
                // Create a background layer.
                BitmapLayer layer = Layer.CreateBackgroundLayer(width, height);

                for (int i = 0; i < resx3; i++)
                {
                    int x = i % width;
                    int y = i / width;

                    byte[] color = reader.ReadBytes(3);
                    ColorBgra surfColor = layer.Surface[x, y];

                    surfColor.R = color[0];
                    surfColor.G = color[1];
                    surfColor.B = color[2];
                    surfColor.A = 255;

                    layer.Surface[x, y] = surfColor;
                }

                // Add the new layer to the Document.
                doc.Layers.Add(layer);
            }

            return doc;
        }
    }
}
