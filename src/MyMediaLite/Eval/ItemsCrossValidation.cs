// Copyright (C) 2011 Zeno Gantner
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
using System.Threading.Tasks;
using MyMediaLite.Data;
using MyMediaLite.ItemRecommendation;

namespace MyMediaLite.Eval
{
	/// <summary>Cross-validation for item recommendation</summary>
	public static class ItemsCrossValidation
	{
		/// <summary>Evaluate on the folds of a dataset split</summary>
		/// <param name="recommender">an item recommender</param>
		/// <param name="split">a dataset split</param>
		/// <param name="test_users">a collection of integers with all test users</param>
		/// <param name="candidate_items">a collection of integers with all candidate items</param>
		/// <param name="candidate_item_mode">the mode used to determine the candidate items</param>
		/// <param name="show_results">set to true to print results to STDERR</param>
		/// <returns>a dictionary containing the average results over the different folds of the split</returns>
		static public Dictionary<string, double> Evaluate(
			ItemRecommender recommender,
			ISplit<IPosOnlyFeedback> split,
			IList<int> test_users,
			IList<int> candidate_items,
			CandidateItems candidate_item_mode = CandidateItems.OVERLAP,
			bool show_results = false)
		{
			var avg_results = new Dictionary<string, double>();

			Parallel.For(0, (int) split.NumberOfFolds, fold =>
			{
				var split_recommender = (ItemRecommender) recommender.Clone(); // avoid changes in recommender
				split_recommender.Feedback = split.Train[fold];
				split_recommender.Train();
				var fold_results = Items.Evaluate(split_recommender, split.Train[fold], split.Test[fold], test_users, candidate_items, candidate_item_mode);

				foreach (var key in fold_results.Keys)
					if (avg_results.ContainsKey(key))
						avg_results[key] += fold_results[key];
					else
						avg_results[key] = fold_results[key];
				if (show_results)
					Console.Error.WriteLine("fold {0} {1}", fold, Items.FormatResults(fold_results));
			});

			foreach (var key in Items.Measures)
				avg_results[key] /= split.NumberOfFolds;
			avg_results["num_users"] /= split.NumberOfFolds;
			avg_results["num_items"] /= split.NumberOfFolds;

			return avg_results;
		}
	}
}

