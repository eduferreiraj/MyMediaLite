// Copyright (C) 2011, 2012 Zeno Gantner
//
// This file is part of MyMediaLite.
//
// MyMediaLite is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MyMediaLite is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with MyMediaLite.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Globalization;

namespace MyMediaLite.Eval
{
	/// <summary>Item recommendation evaluation results</summary>
	/// <remarks>
	/// This class is basically a Dictionary with a custom-made ToString() method.
	/// </remarks>
	[Serializable]
	public class ItemRecommendationEvaluationResults : Dictionary<string, float>
	{
		/// <summary>default constructor</summary>
		public ItemRecommendationEvaluationResults()
		{
			foreach (string method in Items.Measures)
				this[method] = 0;
		}

		/// <summary>Create averaged results</summary>
		/// <param name='result_list'>the list of results to average</param>
		public ItemRecommendationEvaluationResults(IList<Dictionary<string, float>> result_list)
		{
			foreach (var key in result_list[0].Keys)
			{
				this[key] = 0;
				foreach (var r in result_list)
					this[key] += r[key];
				this[key] /= result_list.Count;
			}
		}

		/// <summary>Format item prediction results</summary>
		/// <returns>a string containing the results</returns>
		public override string ToString()
		{
			string s = string.Format(
				CultureInfo.InvariantCulture, "AUC {0:0.#####} prec@1 {1:0.#####} prec@5 {2:0.#####} prec@10 {3:0.#####} MAP {4:0.#####} recall@1 {5:0.#####} recall@5 {6:0.#####} recall@10 {7:0.#####} NDCG {8:0.#####} MRR {9:0.#####} num_users {10} num_items {11} num_lists {12}",
				this["AUC"], this["prec@1"], this["prec@5"], this["prec@10"], this["MAP"], this["recall@1"], this["recall@5"], this["recall@10"], this["NDCG"], this["MRR"], this["num_users"], this["num_items"], this["num_lists"]
			);
			if (this.ContainsKey("fit"))
				s += string.Format(CultureInfo.InvariantCulture, " fit {0:0.#####}", this["fit"]);
			return s;
		}
	}
}

