using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace guessit
{
	public class Words
	{
		private List<string> wordList;
		
		public Words()
		{
			this.wordList = FileSystem.Mounted.ReadJson<List<string>>( "data/words.json" );
			this.wordList = this.wordList.Select(x => x.ToLowerInvariant()).ToList();
			Log.Info( $"Loaded {this.wordList.Count} words!" );
		}

		private string GetRandomWord()
		{
			var rand = new Random();
			return this.wordList[rand.Next( 0, this.wordList.Count )];
		}

		public List<string> GetCandidates()
		{
			var retWords = new List<string>();

			while ( retWords.Count < 3 )
			{
				var candidate = this.GetRandomWord();
				if (retWords.Any(x => x == candidate))
					continue;
				
				retWords.Add( candidate );
			}

			return retWords;
		}

		public static string GetWordWithPlaceholders( string word, int hintAt = -1 )
		{
			var outstr = string.Empty;

			for (var i = 0; i < word.Length; i++)
			{
				if ( hintAt == i )
				{
					outstr += word[i];
					continue;
				}

				outstr += "_";
			}

			return outstr;
		}
	}
}
