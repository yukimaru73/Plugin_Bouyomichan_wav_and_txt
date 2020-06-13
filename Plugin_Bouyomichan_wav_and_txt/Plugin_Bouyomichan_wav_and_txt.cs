using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using FNF.Utility;
using FNF.Controls;
using FNF.XmlSerializerSetting;
using FNF.BouyomiChanApp;

namespace Plugin_Bouyomichan_wav_and_txt
{
	public class Plugin_Bouyomichan_wav_and_txt : IPlugin
	{

		private Settings_wat _Settings;
		private SettingFormData_wat _SettingFormData;
		private string _SettingFile = Base.CallAsmPath + Base.CallAsmName + ".setting";
		private ToolStripButton _Button;
		private ToolStripSeparator _Separator;


		public string Name { get { return "音声 テキスト出力"; } }

		public string Version { get { return "0.1"; } }

		public string Caption { get { return "入力されたテキストを、同名の音声とテキストファイルに出力します。"; } }

		public ISettingFormData SettingFormData { get { return _SettingFormData; } }

		public void Begin()
		{

			_Settings = new Settings_wat(this);
			_Settings.Load(_SettingFile);
			_SettingFormData = new SettingFormData_wat(_Settings);

			_Separator = new ToolStripSeparator();
			Pub.ToolStrip.Items.Add(_Separator);
			_Button = new ToolStripButton(Properties.Resources.wav_and_txt);
			_Button.ToolTipText = "音声とテキストを保存。";
			_Button.Click += Button_Click;
			Pub.ToolStrip.Items.Add(_Button);

			return;
		}

		public void End()
		{
			_Settings.Save(_SettingFile);
			return;
		}

		private void Button_Click(object sender, EventArgs e)
		{
			string textSample = Pub.FormMain.textBoxSource.Text;

			VoiceType voicetype = FNF.Utility.VoiceType.Default;
			int volume = Pub.FormMain.trackBarVolume.Value;	
			int speed = Pub.FormMain.trackBarSpeed.Value;	
			int tone = Pub.FormMain.trackBarTone.Value;
			string comboBoxVoiceType = Pub.FormMain.comboBoxVoiceType.SelectedItem.ToString();

			string FilePath = _SettingFormData.PBase.FilePath;
			DateTime dt = DateTime.Now;

			string Speaker = comboBoxVoiceType + "_V@" + volume.ToString() + "_S@" + speed.ToString() + "_T@" + tone.ToString();
			string FileName = dt.ToString("yyMMdd_HHmmss") + "_" + Speaker +  "_" + textSample;

			StreamWriter sw = new StreamWriter(Path.Combine(@FilePath, FileName + ".txt"), false, Encoding.GetEncoding("shift_jis"));
			sw.Write(textSample);
			sw.Close();

			Pub.AddTalkTask(textSample, speed, tone, volume, voicetype, Path.Combine(FilePath, FileName + ".wav"));
			return;
		}

		public class Settings_wat : SettingsBase
		{
			public string FilePath = @"c:\";

			internal Plugin_Bouyomichan_wav_and_txt Plugin;

			public Settings_wat()
			{

			}

			public Settings_wat(Plugin_Bouyomichan_wav_and_txt pwat)
			{
				Plugin = pwat;
			}

			public override void ReadSettings()
			{

			}

			public override void WriteSettings()
			{

			}
		}

		public class SettingFormData_wat : ISettingFormData
		{
			Settings_wat _Setting;

			public string Title { get { return _Setting.Plugin.Name; } }
			public bool ExpandAll { get { return false; } }
			public SettingsBase Setting { get { return _Setting; } }

			public SettingFormData_wat(Settings_wat setting)
			{
				_Setting = setting;
				PBase = new SBase(_Setting);
			}


			public SBase PBase;
			public class SBase : ISettingPropertyGrid
			{
				Settings_wat _Setting;
				public SBase(Settings_wat setting) { _Setting = setting; }
				public string GetName() { return "音声/テキスト保存"; }

				[Category("基本設定")]
				[DisplayName("01)ファイルの保存場所")]
				[Description("音声とテキストファイルの保存場所を指定します。")]
				[Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
				public string FilePath
				{
					get
					{
						return _Setting.FilePath;
					}
					set
					{
						_Setting.FilePath = value;
					}
				}

				/* ISettingPropertyGridでは設定画面での表示項目を指定できます。
				[Category   ("分類")]
				[DisplayName("表示名")]
				[Description("説明文")]
				[DefaultValue(0)]        //デフォルト値：強調表示されないだけ
				[Browsable(false)]       //PropertyGridで表示しない
				[ReadOnly(true)]         //PropertyGridで読み込み専用にする
				string  ファイル選択     →[Editor(typeof(System.Windows.Forms.Design.FolderNameEditor),       typeof(System.Drawing.Design.UITypeEditor))]
				string  フォルダ選択     →[Editor(typeof(System.Windows.Forms.Design.FileNameEditor),         typeof(System.Drawing.Design.UITypeEditor))]
				string  複数行文字列入力 →[Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
				*/
			}
		}
	}
}