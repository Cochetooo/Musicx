package fr.xahla.musicx.desktop.logging;

import java.io.OutputStream;
import java.io.PrintStream;

/** <b>Class that defines the PrintStream output for logging.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 * @see OutputStream
 */
public class DesktopStream extends OutputStream {
    private PrintStream out;

    public DesktopStream() {
        out = System.out;
    }

    @Override public void write(int i) {
        out.write(i);
    }
}
