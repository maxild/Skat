using System;
using System.Diagnostics;

namespace Maxfire.Skat.UnitTests
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Skatteyder : ISkatteyder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Født: {Foedselsdato: yyyy-MM-dd}, MF: {MedlemAfFolkekirken}, #Børn: {AntalBoern}.";

        public Skatteyder(DateTime foedselsdato, MedlemAfFolkekirken medlemAfFolkekirken)
        {
            Foedselsdato = foedselsdato;
            MedlemAfFolkekirken = medlemAfFolkekirken;
        }

        public Skatteyder(DateTime foedselsdato, MedlemAfFolkekirken medlemAfFolkekirken, AntalBoern antalBoern)
            : this(foedselsdato, medlemAfFolkekirken)
        {
            AntalBoern = antalBoern;
        }

        public MedlemAfFolkekirken MedlemAfFolkekirken { get; }

        public DateTime Foedselsdato { get; }

        public AntalBoern AntalBoern { get; }
    }
}
