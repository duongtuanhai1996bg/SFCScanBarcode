namespace SFCScanBarcode
{
    partial class LotNoForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtLotName = new System.Windows.Forms.TextBox();
            this.txtLotQty = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(94, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nhập số lô/批次号:";
            // 
            // txtLotName
            // 
            this.txtLotName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLotName.Location = new System.Drawing.Point(63, 33);
            this.txtLotName.Name = "txtLotName";
            this.txtLotName.Size = new System.Drawing.Size(210, 20);
            this.txtLotName.TabIndex = 1;
            // 
            // txtLotQty
            // 
            this.txtLotQty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLotQty.Location = new System.Drawing.Point(104, 88);
            this.txtLotQty.Name = "txtLotQty";
            this.txtLotQty.Size = new System.Drawing.Size(126, 20);
            this.txtLotQty.TabIndex = 3;
            this.txtLotQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtLotQty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLotQty_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(75, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nhập số lượng/批次数量:";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.Location = new System.Drawing.Point(121, 118);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(79, 44);
            this.btnConfirm.TabIndex = 4;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // LotNoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 170);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtLotQty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLotName);
            this.Controls.Add(this.label1);
            this.Name = "LotNoForm";
            this.Text = "LotNoForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLotName;
        private System.Windows.Forms.TextBox txtLotQty;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConfirm;
    }
}