public class MainTest {

    public static void main(final String[] args) {
        try {
            final var x = 5 / 0;
            System.out.println("A");
        } catch (final ArithmeticException e) {
            System.out.println("B");
        } finally {
            System.out.println("C");
        }
    }

}
