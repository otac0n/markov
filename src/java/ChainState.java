package markov;

public class ChainState<T>
{
    private final T[] items;

    public ChainState(T[] items) {
        if (items == null)
        {
            throw new IllegalArgumentException("items");
        }

        this.items = items;
    }

    public ChainState<T> push(T item, int order) {
        if (order < 0) {
            throw new IllegalArgumentException("length");
        } else if (order == 0) {
            return this;
        } else if (order == 1) {
            return new ChainState(new Object[] { item });
        } else if (this.items.length < order) {
            Object[] arr = new Object[this.items.length + 1];
            System.arraycopy(this.items, 0, arr, 0, this.items.length);
            arr[this.items.length] = item;
            return new ChainState(arr);
        } else {
            Object[] arr = new Object[order];
            System.arraycopy(this.items, 1, arr, 0, order - 1);
            arr[order - 1] = item;
            return new ChainState(arr);
        }
    }

    @Override
    public int hashCode() {
        int code = this.items.length;

        for (int i = 0; i < this.items.length; i++)
        {
            code ^= this.items[i].hashCode();
        }

        return code;
    }

    @Override
    public boolean equals(Object obj) {
        if (!(obj instanceof ChainState))
        {
            return false;
        }

        return this.equals((ChainState<T>)obj);
    }

    public static boolean equals(ChainState a, ChainState b) {
        if (a == null)
        {
            return b == null;
        }
        else
        {
            return a.equals(b);
        }
    }

    public boolean equals(ChainState<T> other) {
        if (other == null)
        {
            return false;
        }

        if (this.items.length != other.items.length)
        {
            return false;
        }

        for (int i = 0; i < this.items.length; i++)
        {
            if (!this.items[i].equals(other.items[i]))
            {
                return false;
            }
        }

        return true;
    }
}
