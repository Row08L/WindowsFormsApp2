
namespace WindowsFormsApp2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ticks = new System.Windows.Forms.Timer(this.components);
            this.Test = new System.Windows.Forms.Label();
            this.Test2 = new System.Windows.Forms.Label();
            this.test3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ticks
            // 
            this.ticks.Enabled = true;
            this.ticks.Interval = 20;
            this.ticks.Tick += new System.EventHandler(this.ticks_Tick);
            // 
            // Test
            // 
            this.Test.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Test.BackColor = System.Drawing.Color.DimGray;
            this.Test.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Test.ForeColor = System.Drawing.Color.White;
            this.Test.Location = new System.Drawing.Point(1290, 9);
            this.Test.Name = "Test";
            this.Test.Size = new System.Drawing.Size(252, 397);
            this.Test.TabIndex = 0;
            this.Test.Text = "label1";
            this.Test.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Test2
            // 
            this.Test2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Test2.BackColor = System.Drawing.Color.DimGray;
            this.Test2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Test2.ForeColor = System.Drawing.Color.White;
            this.Test2.Location = new System.Drawing.Point(1032, 9);
            this.Test2.Name = "Test2";
            this.Test2.Size = new System.Drawing.Size(252, 397);
            this.Test2.TabIndex = 1;
            this.Test2.Text = "label1";
            this.Test2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // test3
            // 
            this.test3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.test3.BackColor = System.Drawing.Color.DimGray;
            this.test3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.test3.ForeColor = System.Drawing.Color.White;
            this.test3.Location = new System.Drawing.Point(774, 9);
            this.test3.Name = "test3";
            this.test3.Size = new System.Drawing.Size(252, 397);
            this.test3.TabIndex = 2;
            this.test3.Text = "label1";
            this.test3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1719, 450);
            this.Controls.Add(this.test3);
            this.Controls.Add(this.Test2);
            this.Controls.Add(this.Test);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer ticks;
        private System.Windows.Forms.Label Test;
        private System.Windows.Forms.Label Test2;
        private System.Windows.Forms.Label test3;
    }
}

