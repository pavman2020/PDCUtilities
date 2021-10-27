using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace PDCUtility
{
    public class UI
    {
        public static void AlignMiddles(Control c1, Control c2)
        {
            c2.Top = c1.Top + (c1.Height - c2.Height) / 2;
        }
        public static void AlignMiddles(Control c1, Control c2, Control c3)
        {
            AlignMiddles(c1, c2);
            AlignMiddles(c1, c3);
        }

        public static DialogResult ShowInputDialog(string strPrompt, ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = strPrompt;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        public static DialogResult InputMessagebox(String caption
            , String prompt
            , ref String value
            , EventHandler textChangeHandler = null
            )
        {
            /// <summary>
            /// Multiplies two 32-bit values and then divides the 64-bit result by a
            /// third 32-bit value. The final result is rounded to the nearest integer.
            /// </summary>
            Func<int, float, int, int> MulDiv = new Func<int, float, int, int>((nNumber, nNumerator, nDenominator) =>
            {
                return (int)System.Math.Round((float)nNumber * nNumerator / nDenominator);
            });

            Func<Size, float, float, Size> ScaleSize = new Func<Size, float, float, Size>((s, fx, fy) =>
            {
                return new Size((int)(s.Width * fx), (int)(s.Height * fy));
            });

            Form ofrm;
            ofrm = new Form();
            ofrm.AutoScaleMode = AutoScaleMode.Font;
            ofrm.Font = SystemFonts.IconTitleFont;

            SizeF dialogUnits;
            dialogUnits = ofrm.AutoScaleDimensions;

            ofrm.FormBorderStyle = FormBorderStyle.FixedDialog;
            ofrm.MinimizeBox = false;
            ofrm.MaximizeBox = false;
            ofrm.Text = caption;

            ofrm.ClientSize = new Size(MulDiv(180, dialogUnits.Width, 4), MulDiv(63, dialogUnits.Height, 8));

            ofrm.StartPosition = FormStartPosition.CenterScreen;

            System.Windows.Forms.Label lblPrompt;
            lblPrompt = new System.Windows.Forms.Label();
            lblPrompt.Parent = ofrm;
            lblPrompt.AutoSize = true;
            lblPrompt.Left = MulDiv(8, dialogUnits.Width, 4);
            lblPrompt.Top = MulDiv(8, dialogUnits.Height, 8);
            lblPrompt.Text = prompt;

            System.Windows.Forms.TextBox tbInput;
            tbInput = new System.Windows.Forms.TextBox();
            tbInput.Parent = ofrm;
            tbInput.Left = lblPrompt.Left;
            tbInput.Top = MulDiv(19, dialogUnits.Height, 8);
            tbInput.Width = MulDiv(164, dialogUnits.Width, 4);
            tbInput.Text = value;
            tbInput.SelectAll();

            if (null != textChangeHandler)
                tbInput.TextChanged += textChangeHandler;

            int buttonTop = MulDiv(41, dialogUnits.Height, 8);

            //Command buttons should be 50x14 dlus
            Size buttonSize = ScaleSize(new Size(50, 14), dialogUnits.Width / 4, dialogUnits.Height / 8);

            System.Windows.Forms.Button btnOk = new System.Windows.Forms.Button();
            btnOk.Parent = ofrm;
            btnOk.Text = "OK";
            btnOk.DialogResult = DialogResult.OK;
            ofrm.AcceptButton = btnOk;
            btnOk.Location = new Point(MulDiv(38, dialogUnits.Width, 4), buttonTop);
            btnOk.Size = buttonSize;

            System.Windows.Forms.Button btnCnacel = new System.Windows.Forms.Button();
            btnCnacel.Parent = ofrm;
            btnCnacel.Text = "Cancel";
            btnCnacel.DialogResult = DialogResult.Cancel;
            ofrm.CancelButton = btnCnacel;
            btnCnacel.Location = new Point(MulDiv(92, dialogUnits.Width, 4), buttonTop);
            btnCnacel.Size = buttonSize;

            DialogResult dr = ofrm.ShowDialog();

            if (dr == DialogResult.OK)
                value = tbInput.Text;

            return dr;
        }

    }
}