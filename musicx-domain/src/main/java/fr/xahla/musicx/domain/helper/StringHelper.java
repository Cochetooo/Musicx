package fr.xahla.musicx.domain.helper;

public final class StringHelper {

    public static long parseLong(final String str) {
        try {
            return Long.parseLong(str);
        } catch (final NumberFormatException e) {
            return 0L;
        }
    }

    public static int parseInt(final String str) {
        try {
            return Integer.parseInt(str);
        } catch (final NumberFormatException e) {
            return 0;
        }
    }

    public static short parseShort(final String str) {
        try {
            return Short.parseShort(str);
        } catch (final NumberFormatException e) {
            return 0;
        }
    }

}
