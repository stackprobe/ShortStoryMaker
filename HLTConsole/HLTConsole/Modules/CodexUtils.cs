using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLTStudio.Commons;

namespace HLTStudio.Modules
{
	public static class CodexUtils
	{
		public static void Run(string prompt, Func<string, string> promptFilter)
		{
			prompt = promptFilter(prompt);

			string promptFile = Path.Combine(Consts.WORK_DIR, "Prompt.md");

			SCommon.DeletePath(promptFile);
			File.WriteAllText(promptFile, prompt, Encoding.UTF8);

			SCommon.Batch(new string[]
			{
				$"Codex -C \"{Consts.WORK_DIR}\" exec --skip-git-repo-check --sandbox workspace-write \"{promptFile} を読み、その内容を今回の作業指示として実行してください。\"",
			}
			, Consts.WORK_DIR
			, SCommon.StartProcessWindowStyle_e.COEXISTENCE
			);
		}
	}
}
