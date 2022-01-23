using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SourceEngineRAW
{
    internal partial class SourceEngineRAWSaveConfigWidget : SaveConfigWidget
    {
        private CheckBox AllowAnyResolutionBox;

        public SourceEngineRAWSaveConfigWidget()
        {
            AllowAnyResolutionBox = new CheckBox
            {
                AutoSize = true,
                Text = "Allow any resolution"
            };
            AllowAnyResolutionBox.CheckedChanged += tokenChanged;

            Controls.Add(AllowAnyResolutionBox);
        }

        protected override void InitTokenFromWidget()
        {
            SourceEngineRAWSaveConfigToken token = (SourceEngineRAWSaveConfigToken)this.Token;
            token.AllowAnyResolution = this.AllowAnyResolutionBox.Checked;
        }

        protected override void InitWidgetFromToken(SaveConfigToken sourceToken)
        {
            SourceEngineRAWSaveConfigToken token = (SourceEngineRAWSaveConfigToken)sourceToken;
            this.AllowAnyResolutionBox.Checked = token.AllowAnyResolution;
        }

        private void tokenChanged(object sender, EventArgs e)
        {
            UpdateToken();
        }
    }
}
