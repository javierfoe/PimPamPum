using UnityEngine;

namespace Bang
{

    public static class Number
    {
        public const int BANGS = 25;
        public const int MISSEDS = 12;
        public const int BEERS = 6;
        public const int PANICS = 4;
        public const int CAT_BALOUS = 4;
        public const int DUELS = 3;
        public const int JAILS = 3;
        public const int SCHOFIELDS = 3;
        public const int GENERAL_STORES = 2;
        public const int MUSTANGS = 2;
        public const int BARRELS = 2;
        public const int VOLCANICS = 2;
        public const int STAGECOACHES = 2;
        public const int INDIANS = 2;
    }

    public class Roles
    {

        public const string SHERIFF_NAME = "SHERIFF";
        private const string DEPUTY_NAME = "DEPUTY";
        private const string OUTLAW_NAME = "OUTLAW";
        private const string RENEGADE_NAME = "RENEGADE";

        public readonly static Color SHERIFF_COLOR = Color.yellow;
        private readonly static Color DEPUTY_COLOR = Color.green;
        private readonly static Color OUTLAW_COLOR = Color.red;
        private readonly static Color RENEGADE_COLOR = Color.blue;

        public static string GetNameFromRole(ERole role)
        {
            switch (role)
            {
                case ERole.OUTLAW:
                    return OUTLAW_NAME;
                case ERole.DEPUTY:
                    return DEPUTY_NAME;
                case ERole.RENEGADE:
                    return RENEGADE_NAME;
            }
            return null;
        }

        public static ERole[] GetRoles(int players)
        {
            ERole[] result = new ERole[players];
            switch (players)
            {
                case 8:
                    result[7] = ERole.RENEGADE;
                    goto case 7;
                case 7:
                    result[6] = ERole.DEPUTY;
                    goto case 6;
                case 6:
                    result[5] = ERole.OUTLAW;
                    goto case 5;
                case 5:
                    result[4] = ERole.DEPUTY;
                    goto case 4;
                case 4:
                    result[3] = ERole.OUTLAW;
                    goto case 3;
                case 3:
                    result[2] = ERole.OUTLAW;
                    goto case 2;
                case 2:
                    result[1] = ERole.RENEGADE;
                    result[0] = ERole.SHERIFF;
                    break;
            }
            return result;
        }

        public static Color GetColorFromRole(ERole role)
        {
            switch (role)
            {
                case ERole.OUTLAW:
                    return OUTLAW_COLOR;
                case ERole.DEPUTY:
                    return DEPUTY_COLOR;
                case ERole.RENEGADE:
                    return RENEGADE_COLOR;
            }
            return new Color();
        }

    }

    public enum ERole
    {
        SHERIFF,
        DEPUTY,
        OUTLAW,
        RENEGADE
    }

    public enum ECardType
    {
        BANG,
        BARREL,
        BEER,
        BINOCULARS,
        CARABINE,
        CATBALOU,
        COLT45,
        DUEL,
        DYNAMITE,
        GATLING,
        GENERAL_STORE,
        INDIANS,
        JAIL,
        MISSED,
        MUSTANG,
        PANIC,
        REMINGTON,
        SALOON,
        SCHOFIELD,
        STAGECOACH,
        VOLCANIC,
        WELLSFARGO,
        WINCHESTER
    }

    public enum ESuit
    {
        NULL,
        SPADES,
        HEARTS,
        DIAMONDS,
        CLUBS
    }

    public enum ERank
    {
        NULL,
        ACE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JACK,
        QUEEN,
        KING
    }

}
