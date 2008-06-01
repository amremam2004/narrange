namespace NArrange.Gui.Configuration
{
	partial class ConfigurationEditorForm
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
			this._configurationEditorControl = new NArrange.Gui.Configuration.ConfigurationEditorControl();
			this._labelConfigurationFile = new System.Windows.Forms.Label();
			this._configurationPicker = new NArrange.Gui.Configuration.ConfigurationPicker();
			this._buttonSave = new System.Windows.Forms.Button();
			this._buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _configurationEditorControl
			// 
			this._configurationEditorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._configurationEditorControl.Enabled = false;
			this._configurationEditorControl.Location = new System.Drawing.Point(3, 33);
			this._configurationEditorControl.Name = "_configurationEditorControl";
			this._configurationEditorControl.Size = new System.Drawing.Size(727, 327);
			this._configurationEditorControl.TabIndex = 2;
			// 
			// _labelConfigurationFile
			// 
			this._labelConfigurationFile.AutoSize = true;
			this._labelConfigurationFile.Location = new System.Drawing.Point(12, 9);
			this._labelConfigurationFile.Name = "_labelConfigurationFile";
			this._labelConfigurationFile.Size = new System.Drawing.Size(85, 13);
			this._labelConfigurationFile.TabIndex = 0;
			this._labelConfigurationFile.Text = "Configuration file";
			// 
			// _configurationPicker
			// 
			this._configurationPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._configurationPicker.Location = new System.Drawing.Point(103, 6);
			this._configurationPicker.Name = "_configurationPicker";
			this._configurationPicker.SelectedFile = "";
			this._configurationPicker.Size = new System.Drawing.Size(619, 23);
			this._configurationPicker.TabIndex = 1;
			this._configurationPicker.EditClick += new System.EventHandler(this.ConfigurationPickerEditClickHandler);
			this._configurationPicker.CreateClick += new System.EventHandler(this.ButtonCreateClickHandler);
			// 
			// _buttonSave
			// 
			this._buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonSave.Enabled = false;
			this._buttonSave.Location = new System.Drawing.Point(566, 369);
			this._buttonSave.Name = "_buttonSave";
			this._buttonSave.Size = new System.Drawing.Size(75, 23);
			this._buttonSave.TabIndex = 3;
			this._buttonSave.Text = "&Save";
			this._buttonSave.UseVisualStyleBackColor = true;
			this._buttonSave.Click += new System.EventHandler(this.ButtonSaveClickHandler);
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonCancel.Enabled = false;
			this._buttonCancel.Location = new System.Drawing.Point(647, 369);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(75, 23);
			this._buttonCancel.TabIndex = 4;
			this._buttonCancel.Text = "&Cancel";
			this._buttonCancel.UseVisualStyleBackColor = true;
			this._buttonCancel.Click += new System.EventHandler(this.ButtonCancelClickHandler);
			// 
			// ConfigurationEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(734, 404);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonSave);
			this.Controls.Add(this._configurationPicker);
			this.Controls.Add(this._labelConfigurationFile);
			this.Controls.Add(this._configurationEditorControl);
			this.Name = "ConfigurationEditorForm";
			this.Text = "NArrange - Configuration Editor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ConfigurationEditorControl _configurationEditorControl;
		private System.Windows.Forms.Label _labelConfigurationFile;
		private ConfigurationPicker _configurationPicker;
		private System.Windows.Forms.Button _buttonSave;
		private System.Windows.Forms.Button _buttonCancel;
	}
}