﻿namespace Polymorphie
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblAnzeige = new System.Windows.Forms.Label();
            this.cmdAnzeigen = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAnzeige
            // 
            this.lblAnzeige.AutoSize = true;
            this.lblAnzeige.Location = new System.Drawing.Point(12, 21);
            this.lblAnzeige.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAnzeige.Name = "lblAnzeige";
            this.lblAnzeige.Size = new System.Drawing.Size(0, 17);
            this.lblAnzeige.TabIndex = 7;
            // 
            // cmdAnzeigen
            // 
            this.cmdAnzeigen.Location = new System.Drawing.Point(263, 15);
            this.cmdAnzeigen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdAnzeigen.Name = "cmdAnzeigen";
            this.cmdAnzeigen.Size = new System.Drawing.Size(100, 28);
            this.cmdAnzeigen.TabIndex = 6;
            this.cmdAnzeigen.Text = "Anzeigen";
            this.cmdAnzeigen.UseVisualStyleBackColor = true;
            this.cmdAnzeigen.Click += new System.EventHandler(this.cmdAnzeigen_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 335);
            this.Controls.Add(this.lblAnzeige);
            this.Controls.Add(this.cmdAnzeigen);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Polymorphie";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label lblAnzeige;
        internal System.Windows.Forms.Button cmdAnzeigen;
    }
}

