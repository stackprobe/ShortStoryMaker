using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HLTStudio
{
	public static class Consts
	{
		/// <summary>
		/// 作業ディレクトリ
		/// </summary>
		public static string WORK_DIR => @"C:\temp\_SSM";

		/// <summary>
		/// データ保管庫ディレクトリ
		/// </summary>
		public static string STORAGE_DIR => @"C:\home\Chest\20260719_ShortStoryMaker\Storage";

		/// <summary>
		/// プロット置き場ディレクトリ
		/// </summary>
		public static string PLOT_STORAGE_DIR => Path.Combine(STORAGE_DIR, "Plot");

		/// <summary>
		/// マテリアル置き場ディレクトリ
		/// </summary>
		public static string MATERIAL_STORAGE_DIR => Path.Combine(STORAGE_DIR, "Material");

		/// <summary>
		/// ストーリー置き場ディレクトリ
		/// </summary>
		public static string STORY_STORAGE_DIR => Path.Combine(STORAGE_DIR, "Story");

		/// <summary>
		/// イラスト置き場ディレクトリ
		/// </summary>
		public static string ILLUST_STORAGE_DIR => Path.Combine(STORAGE_DIR, "Illust");
	}
}
