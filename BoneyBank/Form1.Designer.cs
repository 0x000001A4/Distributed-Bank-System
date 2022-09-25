namespace BoneyBank
{
    partial class ClientForm
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
            this.ClientIdLabelStatic = new System.Windows.Forms.Label();
            this.BalanceLabelStatic = new System.Windows.Forms.Label();
            this.TransactionLogTxtBox = new System.Windows.Forms.RichTextBox();
            this.TransactionLogLabel = new System.Windows.Forms.Label();
            this.FrozenRadioBtn = new System.Windows.Forms.RadioButton();
            this.Withdrawgroup = new System.Windows.Forms.GroupBox();
            this.WithdrawAmountTxtBox = new System.Windows.Forms.TextBox();
            this.WithdrawAmountLabel = new System.Windows.Forms.Label();
            this.TransactionGroup = new System.Windows.Forms.GroupBox();
            this.TransactionAmountTxtBox = new System.Windows.Forms.TextBox();
            this.DestBankIdTxtBox = new System.Windows.Forms.TextBox();
            this.TransactionAmountLabel = new System.Windows.Forms.Label();
            this.DestBankIdLabel = new System.Windows.Forms.Label();
            this.TransactionBtn = new System.Windows.Forms.Button();
            this.AccountGroup = new System.Windows.Forms.GroupBox();
            this.DepositGroup = new System.Windows.Forms.GroupBox();
            this.DepositAmountTxtBox = new System.Windows.Forms.TextBox();
            this.DepositAmountLabel = new System.Windows.Forms.Label();
            this.DepositBtn = new System.Windows.Forms.Button();
            this.LoginGroup = new System.Windows.Forms.GroupBox();
            this.LoginBankIdTxtBox = new System.Windows.Forms.TextBox();
            this.LoginBankIdLabel = new System.Windows.Forms.Label();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.WithdrawBtn = new System.Windows.Forms.Button();
            this.Withdrawgroup.SuspendLayout();
            this.TransactionGroup.SuspendLayout();
            this.AccountGroup.SuspendLayout();
            this.DepositGroup.SuspendLayout();
            this.LoginGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClientIdLabelStatic
            // 
            this.ClientIdLabelStatic.AutoSize = true;
            this.ClientIdLabelStatic.Location = new System.Drawing.Point(15, 21);
            this.ClientIdLabelStatic.Name = "ClientIdLabelStatic";
            this.ClientIdLabelStatic.Size = new System.Drawing.Size(50, 13);
            this.ClientIdLabelStatic.TabIndex = 0;
            this.ClientIdLabelStatic.Text = "Client ID:";
            // 
            // BalanceLabelStatic
            // 
            this.BalanceLabelStatic.AutoSize = true;
            this.BalanceLabelStatic.Location = new System.Drawing.Point(16, 43);
            this.BalanceLabelStatic.Name = "BalanceLabelStatic";
            this.BalanceLabelStatic.Size = new System.Drawing.Size(49, 13);
            this.BalanceLabelStatic.TabIndex = 1;
            this.BalanceLabelStatic.Text = "Balance:";
            // 
            // TransactionLogTxtBox
            // 
            this.TransactionLogTxtBox.Location = new System.Drawing.Point(532, 59);
            this.TransactionLogTxtBox.Name = "TransactionLogTxtBox";
            this.TransactionLogTxtBox.Size = new System.Drawing.Size(258, 321);
            this.TransactionLogTxtBox.TabIndex = 5;
            this.TransactionLogTxtBox.Text = "";
            // 
            // TransactionLogLabel
            // 
            this.TransactionLogLabel.AutoSize = true;
            this.TransactionLogLabel.Location = new System.Drawing.Point(529, 34);
            this.TransactionLogLabel.Name = "TransactionLogLabel";
            this.TransactionLogLabel.Size = new System.Drawing.Size(92, 13);
            this.TransactionLogLabel.TabIndex = 6;
            this.TransactionLogLabel.Text = "Transactions Log:";
            // 
            // FrozenRadioBtn
            // 
            this.FrozenRadioBtn.AutoSize = true;
            this.FrozenRadioBtn.Location = new System.Drawing.Point(18, 71);
            this.FrozenRadioBtn.Name = "FrozenRadioBtn";
            this.FrozenRadioBtn.Size = new System.Drawing.Size(57, 17);
            this.FrozenRadioBtn.TabIndex = 13;
            this.FrozenRadioBtn.TabStop = true;
            this.FrozenRadioBtn.Text = "Frozen";
            this.FrozenRadioBtn.UseVisualStyleBackColor = true;
            // 
            // Withdrawgroup
            // 
            this.Withdrawgroup.Controls.Add(this.WithdrawBtn);
            this.Withdrawgroup.Controls.Add(this.WithdrawAmountTxtBox);
            this.Withdrawgroup.Controls.Add(this.WithdrawAmountLabel);
            this.Withdrawgroup.Location = new System.Drawing.Point(29, 274);
            this.Withdrawgroup.Name = "Withdrawgroup";
            this.Withdrawgroup.Size = new System.Drawing.Size(199, 106);
            this.Withdrawgroup.TabIndex = 20;
            this.Withdrawgroup.TabStop = false;
            this.Withdrawgroup.Text = "Withdraw";
            // 
            // WithdrawAmountTxtBox
            // 
            this.WithdrawAmountTxtBox.Location = new System.Drawing.Point(67, 24);
            this.WithdrawAmountTxtBox.Name = "WithdrawAmountTxtBox";
            this.WithdrawAmountTxtBox.Size = new System.Drawing.Size(108, 20);
            this.WithdrawAmountTxtBox.TabIndex = 14;
            // 
            // WithdrawAmountLabel
            // 
            this.WithdrawAmountLabel.AutoSize = true;
            this.WithdrawAmountLabel.Location = new System.Drawing.Point(15, 27);
            this.WithdrawAmountLabel.Name = "WithdrawAmountLabel";
            this.WithdrawAmountLabel.Size = new System.Drawing.Size(46, 13);
            this.WithdrawAmountLabel.TabIndex = 15;
            this.WithdrawAmountLabel.Text = "Amount:";
            // 
            // TransactionGroup
            // 
            this.TransactionGroup.Controls.Add(this.TransactionAmountTxtBox);
            this.TransactionGroup.Controls.Add(this.DestBankIdTxtBox);
            this.TransactionGroup.Controls.Add(this.TransactionAmountLabel);
            this.TransactionGroup.Controls.Add(this.DestBankIdLabel);
            this.TransactionGroup.Controls.Add(this.TransactionBtn);
            this.TransactionGroup.Location = new System.Drawing.Point(250, 151);
            this.TransactionGroup.Name = "TransactionGroup";
            this.TransactionGroup.Size = new System.Drawing.Size(264, 134);
            this.TransactionGroup.TabIndex = 21;
            this.TransactionGroup.TabStop = false;
            this.TransactionGroup.Text = "Transaction";
            // 
            // TransactionAmountTxtBox
            // 
            this.TransactionAmountTxtBox.Location = new System.Drawing.Point(134, 56);
            this.TransactionAmountTxtBox.Name = "TransactionAmountTxtBox";
            this.TransactionAmountTxtBox.Size = new System.Drawing.Size(110, 20);
            this.TransactionAmountTxtBox.TabIndex = 18;
            // 
            // DestBankIdTxtBox
            // 
            this.DestBankIdTxtBox.Location = new System.Drawing.Point(134, 24);
            this.DestBankIdTxtBox.Name = "DestBankIdTxtBox";
            this.DestBankIdTxtBox.Size = new System.Drawing.Size(110, 20);
            this.DestBankIdTxtBox.TabIndex = 14;
            // 
            // TransactionAmountLabel
            // 
            this.TransactionAmountLabel.AutoSize = true;
            this.TransactionAmountLabel.Location = new System.Drawing.Point(64, 59);
            this.TransactionAmountLabel.Name = "TransactionAmountLabel";
            this.TransactionAmountLabel.Size = new System.Drawing.Size(46, 13);
            this.TransactionAmountLabel.TabIndex = 17;
            this.TransactionAmountLabel.Text = "Amount:";
            // 
            // DestBankIdLabel
            // 
            this.DestBankIdLabel.AutoSize = true;
            this.DestBankIdLabel.Location = new System.Drawing.Point(6, 27);
            this.DestBankIdLabel.Name = "DestBankIdLabel";
            this.DestBankIdLabel.Size = new System.Drawing.Size(105, 13);
            this.DestBankIdLabel.TabIndex = 15;
            this.DestBankIdLabel.Text = "Destination Bank ID:";
            // 
            // TransactionBtn
            // 
            this.TransactionBtn.Location = new System.Drawing.Point(134, 92);
            this.TransactionBtn.Name = "TransactionBtn";
            this.TransactionBtn.Size = new System.Drawing.Size(110, 23);
            this.TransactionBtn.TabIndex = 16;
            this.TransactionBtn.Text = "Transfer";
            this.TransactionBtn.UseVisualStyleBackColor = true;
            // 
            // AccountGroup
            // 
            this.AccountGroup.Controls.Add(this.BalanceLabelStatic);
            this.AccountGroup.Controls.Add(this.FrozenRadioBtn);
            this.AccountGroup.Controls.Add(this.ClientIdLabelStatic);
            this.AccountGroup.Location = new System.Drawing.Point(250, 31);
            this.AccountGroup.Name = "AccountGroup";
            this.AccountGroup.Size = new System.Drawing.Size(264, 110);
            this.AccountGroup.TabIndex = 23;
            this.AccountGroup.TabStop = false;
            this.AccountGroup.Text = "Account info:";
            // 
            // DepositGroup
            // 
            this.DepositGroup.Controls.Add(this.DepositAmountTxtBox);
            this.DepositGroup.Controls.Add(this.DepositAmountLabel);
            this.DepositGroup.Controls.Add(this.DepositBtn);
            this.DepositGroup.Location = new System.Drawing.Point(29, 151);
            this.DepositGroup.Name = "DepositGroup";
            this.DepositGroup.Size = new System.Drawing.Size(199, 106);
            this.DepositGroup.TabIndex = 21;
            this.DepositGroup.TabStop = false;
            this.DepositGroup.Text = "Deposit";
            // 
            // DepositAmountTxtBox
            // 
            this.DepositAmountTxtBox.Location = new System.Drawing.Point(67, 24);
            this.DepositAmountTxtBox.Name = "DepositAmountTxtBox";
            this.DepositAmountTxtBox.Size = new System.Drawing.Size(108, 20);
            this.DepositAmountTxtBox.TabIndex = 14;
            // 
            // DepositAmountLabel
            // 
            this.DepositAmountLabel.AutoSize = true;
            this.DepositAmountLabel.Location = new System.Drawing.Point(15, 27);
            this.DepositAmountLabel.Name = "DepositAmountLabel";
            this.DepositAmountLabel.Size = new System.Drawing.Size(46, 13);
            this.DepositAmountLabel.TabIndex = 15;
            this.DepositAmountLabel.Text = "Amount:";
            // 
            // DepositBtn
            // 
            this.DepositBtn.Location = new System.Drawing.Point(68, 50);
            this.DepositBtn.Name = "DepositBtn";
            this.DepositBtn.Size = new System.Drawing.Size(104, 23);
            this.DepositBtn.TabIndex = 16;
            this.DepositBtn.Text = "Deposit";
            this.DepositBtn.UseVisualStyleBackColor = true;
            // 
            // LoginGroup
            // 
            this.LoginGroup.Controls.Add(this.LoginBankIdTxtBox);
            this.LoginGroup.Controls.Add(this.LoginBankIdLabel);
            this.LoginGroup.Controls.Add(this.LoginBtn);
            this.LoginGroup.Location = new System.Drawing.Point(29, 31);
            this.LoginGroup.Name = "LoginGroup";
            this.LoginGroup.Size = new System.Drawing.Size(199, 106);
            this.LoginGroup.TabIndex = 21;
            this.LoginGroup.TabStop = false;
            this.LoginGroup.Text = "Login";
            // 
            // LoginBankIdTxtBox
            // 
            this.LoginBankIdTxtBox.Location = new System.Drawing.Point(67, 24);
            this.LoginBankIdTxtBox.Name = "LoginBankIdTxtBox";
            this.LoginBankIdTxtBox.Size = new System.Drawing.Size(108, 20);
            this.LoginBankIdTxtBox.TabIndex = 14;
            // 
            // LoginBankIdLabel
            // 
            this.LoginBankIdLabel.AutoSize = true;
            this.LoginBankIdLabel.Location = new System.Drawing.Point(15, 27);
            this.LoginBankIdLabel.Name = "LoginBankIdLabel";
            this.LoginBankIdLabel.Size = new System.Drawing.Size(49, 13);
            this.LoginBankIdLabel.TabIndex = 15;
            this.LoginBankIdLabel.Text = "Bank ID:";
            // 
            // LoginBtn
            // 
            this.LoginBtn.Location = new System.Drawing.Point(68, 50);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(104, 23);
            this.LoginBtn.TabIndex = 16;
            this.LoginBtn.Text = "Connect";
            this.LoginBtn.UseVisualStyleBackColor = true;
            // 
            // WithdrawBtn
            // 
            this.WithdrawBtn.Location = new System.Drawing.Point(68, 59);
            this.WithdrawBtn.Name = "WithdrawBtn";
            this.WithdrawBtn.Size = new System.Drawing.Size(104, 23);
            this.WithdrawBtn.TabIndex = 17;
            this.WithdrawBtn.Text = "Withdraw";
            this.WithdrawBtn.UseVisualStyleBackColor = true;
            // 
            // ClientForm
            // 
            this.ClientSize = new System.Drawing.Size(820, 412);
            this.Controls.Add(this.LoginGroup);
            this.Controls.Add(this.DepositGroup);
            this.Controls.Add(this.Withdrawgroup);
            this.Controls.Add(this.TransactionGroup);
            this.Controls.Add(this.AccountGroup);
            this.Controls.Add(this.TransactionLogLabel);
            this.Controls.Add(this.TransactionLogTxtBox);
            this.Name = "ClientForm";
            this.Text = "Boney Bank";
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.Withdrawgroup.ResumeLayout(false);
            this.Withdrawgroup.PerformLayout();
            this.TransactionGroup.ResumeLayout(false);
            this.TransactionGroup.PerformLayout();
            this.AccountGroup.ResumeLayout(false);
            this.AccountGroup.PerformLayout();
            this.DepositGroup.ResumeLayout(false);
            this.DepositGroup.PerformLayout();
            this.LoginGroup.ResumeLayout(false);
            this.LoginGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ClientIdLabelStatic;
        private System.Windows.Forms.Label BalanceLabelStatic;
        private System.Windows.Forms.RichTextBox TransactionLogTxtBox;
        private System.Windows.Forms.Label TransactionLogLabel;
        private System.Windows.Forms.RadioButton FrozenRadioBtn;
        private System.Windows.Forms.GroupBox Withdrawgroup;
        private System.Windows.Forms.Label WithdrawAmountLabel;
        private System.Windows.Forms.TextBox WithdrawAmountTxtBox;
        private System.Windows.Forms.GroupBox TransactionGroup;
        private System.Windows.Forms.TextBox TransactionAmountTxtBox;
        private System.Windows.Forms.Label TransactionAmountLabel;
        private System.Windows.Forms.Label DestBankIdLabel;
        private System.Windows.Forms.TextBox DestBankIdTxtBox;
        private System.Windows.Forms.Button TransactionBtn;
        private System.Windows.Forms.GroupBox AccountGroup;
        private System.Windows.Forms.GroupBox DepositGroup;
        private System.Windows.Forms.TextBox DepositAmountTxtBox;
        private System.Windows.Forms.Label DepositAmountLabel;
        private System.Windows.Forms.Button DepositBtn;
        private System.Windows.Forms.GroupBox LoginGroup;
        private System.Windows.Forms.TextBox LoginBankIdTxtBox;
        private System.Windows.Forms.Label LoginBankIdLabel;
        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.Button WithdrawBtn;
    }
}

