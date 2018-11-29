using UnityEngine;

namespace Bang
{

    public static class Number
    {
        public const int Bangs = 25;
        public const int Misseds = 12;
        public const int Beers = 6;
        public const int Panics = 4;
        public const int CatBalous = 4;
        public const int Duels = 3;
        public const int Jails = 3;
        public const int Schofields = 3;
        public const int GeneralStores = 2;
        public const int Mustangs = 2;
        public const int Barrels = 2;
        public const int Volcanics = 2;
        public const int Stagecoaches = 2;
        public const int Indians = 2;
    }

    public class Roles
    {

        public const string SheriffName = "SHERIFF";
        private const string DeputyName = "DEPUTY";
        private const string OutlawName = "OUTLAW";
        private const string RenegadeName = "RENEGADE";

        public readonly static Color SheriffColor = Color.yellow;
        private readonly static Color DeputyColor = Color.green;
        private readonly static Color OutlawColor = Color.red;
        private readonly static Color RenegadeColor = Color.blue;

        public static string GetNameFromRole(ERole role)
        {
            switch (role)
            {
                case ERole.Outlaw:
                    return OutlawName;
                case ERole.Deputy:
                    return DeputyName;
                case ERole.Renegade:
                    return RenegadeName;
            }
            return null;
        }

        public static ERole[] GetRoles(int players)
        {
            ERole[] result = new ERole[players];
            switch (players)
            {
                case 8:
                    result[7] = ERole.Renegade;
                    goto case 7;
                case 7:
                    result[6] = ERole.Deputy;
                    goto case 6;
                case 6:
                    result[5] = ERole.Outlaw;
                    goto case 5;
                case 5:
                    result[4] = ERole.Deputy;
                    goto case 4;
                case 4:
                    result[3] = ERole.Outlaw;
                    goto case 3;
                case 3:
                    result[2] = ERole.Outlaw;
                    goto case 2;
                case 2:
                    result[1] = ERole.Renegade;
                    result[0] = ERole.Sheriff;
                    break;
            }
            return result;
        }

        public static Color GetColorFromRole(ERole role)
        {
            switch (role)
            {
                case ERole.Outlaw:
                    return OutlawColor;
                case ERole.Deputy:
                    return DeputyColor;
                case ERole.Renegade:
                    return RenegadeColor;
            }
            return new Color();
        }

    }

    public enum EDecision
    {
        Pending,
        TakeHit,
        Dodge,
        Duel,
        Source
    }

    public enum ERole
    {
        Sheriff,
        Deputy,
        Outlaw,
        Renegade
    }

    public enum ECardType
    {
        Bang,
        Barrel,
        Beer,
        Scope,
        Carabine,
        CatBalou,
        Colt45,
        Duel,
        Dynamite,
        Gatling,
        GeneralStore,
        Indians,
        Jail,
        Missed,
        Mustang,
        Panic,
        Remington,
        Saloon,
        Schofield,
        Stagecoach,
        Volcanic,
        WellsFargo,
        Winchester
    }

    public enum ESuit
    {
        Null,
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }

    public enum ERank
    {
        Null,
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

}
