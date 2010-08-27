namespace Maxfire.Skat
{
	/// <summary>
	/// Resultat af enten egne modregninger eller ægtefælle modregninger
	/// </summary>
	public class BeregnModregningerResult
	{
		public BeregnModregningerResult(decimal modregningSkattepligtigIndkomst, 
			SkatterAfPersonligIndkomst modregningSkatter, decimal modregningUnderskud)
		{
			ModregningUnderskudSkattepligtigIndkomst = modregningSkattepligtigIndkomst;
			ModregningSkatter = modregningSkatter;
			ModregningUnderskud = modregningUnderskud;
		}

		/// <summary>
		/// Modregning i den (positive) skattepligtige indkomst.
		/// </summary>
		public decimal ModregningUnderskudSkattepligtigIndkomst { get; private set; }

		/// <summary>
		/// Modregninger i skatterne.
		/// </summary>
		public SkatterAfPersonligIndkomst ModregningSkatter { get; private set; }

		/// <summary>
		/// Modregninger i skatter omregnet til underskud.
		/// </summary>
		public decimal ModregningUnderskudSkatter
		{
			get { return ModregningUnderskud - ModregningUnderskudSkattepligtigIndkomst; }
		}

		/// <summary>
		/// Reduktionen i underskuddet som følge af modregningen i den (positive) skattepligtige indkomst og skatter.
		/// </summary>
		public decimal ModregningUnderskud { get; private set; }

		public decimal GetRestunderskud(decimal underskud)
		{
			return underskud - ModregningUnderskud;
		}

		public static BeregnModregningerResult operator+ (BeregnModregningerResult lhs, BeregnModregningerResult rhs)
		{
			return new BeregnModregningerResult(
				lhs.ModregningUnderskudSkattepligtigIndkomst + rhs.ModregningUnderskudSkattepligtigIndkomst,
				lhs.ModregningSkatter + rhs.ModregningSkatter,
				lhs.ModregningUnderskud + rhs.ModregningUnderskud);
		}
	}

	/// <summary>
	/// Resultat af samlet modregning i egen skattepligtige indkomst og skatter samt ægtefælles skatepligtige indkomst og skatter.
	/// </summary>
	public class ModregnUnderskudResult
	{
		public ModregnUnderskudResult(decimal underskud, decimal modregningUnderskud, decimal modregningSkattepligtigIndkomst, 
			SkatterAfPersonligIndkomst skatter, SkatterAfPersonligIndkomst modregningSkatter)
		{
			Underskud = underskud;
			ModregningUnderskud = modregningUnderskud;
			ModregningUnderskudSkattepligtigIndkomst = modregningSkattepligtigIndkomst;
			Skatter = skatter;
			ModregningSkatter = modregningSkatter;
		}

		/// <summary>
		/// Summen af årets underskud og fremført underskud inden modregning og fremførsel.
		/// </summary>
		public decimal Underskud { get; private set; }

		/// <summary>
		/// Den del af summen af årets underskud og fremført underskud, der er benyttet 
		/// til modregning i enten egen eller ægtefælles (positive) skattepligtige indkomst.
		/// </summary>
		public decimal ModregningUnderskudSkattepligtigIndkomst { get; private set; }

		/// <summary>
		/// Den del af summen af årets underskud og fremført underskud, der er benyttet 
		/// til modregning i enten egen eller ægtefælles skatter.
		/// </summary>
		public decimal ModregningUnderskudSkatter
		{
			get { return ModregningUnderskud - ModregningUnderskudSkattepligtigIndkomst; }
		}

		/// <summary>
		/// Den del af summen af årets underskud og fremført underskud, der er benyttet 
		/// til modregning i enten egen eller ægtefælles (positive) skattepligtige indkomst og skatter.
		/// </summary>
		public decimal ModregningUnderskud { get; private set; }

		/// <summary>
		/// Den del af summen af årets underskud og fremført underskud, der 
		/// skal fremføres til næste indkomstår.
		/// </summary>
		public decimal UnderskudTilFremfoersel
		{
			get { return Underskud - ModregningUnderskud; }
		}

		/// <summary>
		/// Størrelsen af de modregninger af skatteværdier, der kan rummes i skatterne.
		/// </summary>
		public SkatterAfPersonligIndkomst ModregningSkatter { get; private set; }

		/// <summary>
		/// Skatterne inden modregning af underskudsværdi.
		/// </summary>
		public SkatterAfPersonligIndkomst Skatter { get; private set; }

		/// <summary>
		/// Skatterne efter modregning af underskudsværdi.
		/// </summary>
		public SkatterAfPersonligIndkomst ModregnedeSkatter
		{
			get { return Skatter - ModregningSkatter; }
		}
	}
}