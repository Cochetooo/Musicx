package fr.xahla.musicx.desktop.logging;

import java.io.IOException;
import java.io.OutputStream;
import java.io.PrintStream;

public class DesktopStream extends OutputStream {
    private PrintStream out;

    public DesktopStream() {
        out = System.out;
    }

    @Override public void write(int i) throws IOException {
        out.write(i);
    }
}
