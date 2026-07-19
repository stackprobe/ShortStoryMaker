using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using HLTStudio.Commons;
using HLTStudio.Modules;

namespace HLTStudio
{
	class Program
	{
		static void Main(string[] args)
		{
			ProcMain.CUIMain(new Program().Main2);
		}

		private void Main2(ArgsReader ar)
		{
			if (ProcMain.DEBUG)
			{
				Main3();
			}
			else
			{
				Main4(ar);
			}
			SCommon.OpenOutputDirIfCreated();
		}

		private void Main3()
		{
#if DEBUG
			// -- choose one --

			Main4(new ArgsReader(new string[] { }));
			//Main4(new ArgsReader(new string[] { }));
			//Main4(new ArgsReader(new string[] { }));

			// --
#endif
			SCommon.Pause();
		}

		private void Main4(ArgsReader ar)
		{
			try
			{
				Main5(ar);
			}
			catch (Exception ex)
			{
				ProcMain.WriteLog(ex);

				MessageBox.Show(ex.ToString(), $"{Path.GetFileNameWithoutExtension(ProcMain.SelfFile)} / エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Main5(ArgsReader ar)
		{
			SCommon.DeleteAndCreateDir(Consts.WORK_DIR);

			string dekigotoFile = Path.Combine(Consts.WORK_DIR, "Dekigoto.md");
			string materialDir = Path.Combine(Consts.WORK_DIR, "Material");
			string plotFile_01 = Path.Combine(Consts.WORK_DIR, "Plot-01.md");
			string plotFile_02 = Path.Combine(Consts.WORK_DIR, "Plot-02.md");
			string plotFile_03 = Path.Combine(Consts.WORK_DIR, "Plot-03.md");

			SCommon.CreateDir(materialDir);

			CodexUtils.Run(PromptResource.PROMPT_01, prompt =>
			{
				prompt = SCommon.ReplaceAll(
					prompt,
					"{{YEAR_MONTH}}", $"西暦 {SCommon.CRandom.GetRange(1600, 2025)} 年 {SCommon.CRandom.GetRange(1, 12)} 月",
					"{{OUTPUT_FILE}}", dekigotoFile
					);

				return prompt;
			});

			CodexUtils.Run(PromptResource.PROMPT_02, prompt =>
			{
				prompt = SCommon.ReplaceAll(
					prompt,
					"{{INPUT_FILE}}", dekigotoFile,
					"{{OUTPUT_MATERIAL_DIR}}", materialDir,
					"{{OUTPUT_PLOT_FILE_1}}", plotFile_01,
					"{{OUTPUT_PLOT_FILE_2}}", plotFile_02,
					"{{OUTPUT_PLOT_FILE_3}}", plotFile_03
					);

				return prompt;
			});
		}
	}
}
