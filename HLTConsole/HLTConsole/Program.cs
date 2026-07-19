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
			SCommon.CreateDir(Consts.PLOT_STORAGE_DIR);
			SCommon.CreateDir(Consts.MATERIAL_STORAGE_DIR);
			SCommon.CreateDir(Consts.STORY_STORAGE_DIR);

			string[] plotFiles = Directory.GetFiles(Consts.PLOT_STORAGE_DIR);
			string[] materialFiles = Directory.GetFiles(Consts.MATERIAL_STORAGE_DIR);

			if (
				plotFiles.Length < 10 ||
				materialFiles.Length < 10
				)
			{
				素材補充();
			}
			else
			{
				執筆();
			}
		}

		private void 素材補充()
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

			string[] materialFiles = Directory.GetFiles(materialDir, "*.md");

			// -- 出力存在チェック --

			if (materialFiles.Length < 1)
				throw new Exception("マテリアルが出力されていません");

			if (!File.Exists(plotFile_01))
				throw new Exception("プロット(1)が出力されていません");

			if (!File.Exists(plotFile_02))
				throw new Exception("プロット(2)が出力されていません");

			if (!File.Exists(plotFile_03))
				throw new Exception("プロット(3)が出力されていません");

			// --

			foreach (string file in new string[] { plotFile_01, plotFile_02, plotFile_03 })
				File.Move(file, Path.Combine(Consts.PLOT_STORAGE_DIR, SCommon.GetULID() + ".md"));

			foreach (string file in materialFiles)
				File.Move(file, Path.Combine(Consts.MATERIAL_STORAGE_DIR, SCommon.GetULID() + ".md"));
		}

		private void 執筆()
		{
			string[] plotFiles = Directory.GetFiles(Consts.PLOT_STORAGE_DIR);
			string[] materialFiles = Directory.GetFiles(Consts.MATERIAL_STORAGE_DIR);

			if (plotFiles.Length < 1)
				throw new Exception("no plotFiles");

			if (materialFiles.Length < 1)
				throw new Exception("no materialFiles");

			int materialCount = Math.Max(1, materialFiles.Length / plotFiles.Length);

			string selectedPlotFile = SCommon.CRandom.ChooseOne(plotFiles);
			SCommon.CRandom.Shuffle(materialFiles);
			string[] selectedMaterialFiles = SCommon.GetPart(materialFiles, 0, materialCount);

			SCommon.DeleteAndCreateDir(Consts.WORK_DIR);

			string workPlotFile = Path.Combine(Consts.WORK_DIR, "Plot.md");
			string[] workMaterialFiles = selectedMaterialFiles
				.Select(file => Path.Combine(Consts.WORK_DIR, Path.GetFileName(file)))
				.ToArray();
			string outputFile = Path.Combine(Consts.WORK_DIR, "Output.txt");

			File.Copy(selectedPlotFile, workPlotFile);

			for (int i = 0; i < materialCount; i++)
				File.Copy(selectedMaterialFiles[i], workMaterialFiles[i]);

			CodexUtils.Run(PromptResource.PROMPT_03, prompt =>
			{
				prompt = SCommon.ReplaceAll(
					prompt,
					"{{INPUT_PLOT_FILE}}", workPlotFile,
					"{{INPUT_MATERIAL_FILES}}", string.Join("\r\n", workMaterialFiles),
					"{{OUTPUT_FILE}}", outputFile
					);

				return prompt;
			});

			if (!File.Exists(outputFile))
				throw new Exception("出力ファイルが作成されませんでした。");

			File.Move(outputFile, Path.Combine(Consts.STORY_STORAGE_DIR, SCommon.GetULID() + ".txt"));

			SCommon.DeletePath(selectedPlotFile);

			foreach (string file in selectedMaterialFiles)
				SCommon.DeletePath(file);
		}
	}
}
