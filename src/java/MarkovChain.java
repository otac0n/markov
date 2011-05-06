package markov;

import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.Random;
import java.util.Arrays;

public class MarkovChain<T>
{
    public final int order;

    private final HashMap<ChainState<T>, HashMap<T, Integer>> items = new HashMap<ChainState<T>, HashMap<T, Integer>>();
    private final HashMap<ChainState<T>, Integer> terminals = new HashMap<ChainState<T>, Integer>();

    public MarkovChain(int order) {
        if (order < 0)
        {
            throw new IllegalArgumentException("order");
        }

        this.order = order;
    }

    public void add(Iterable<T> items) {
        this.add(items, 1);
    }

    public void add(T[] items) {
        this.add(Arrays.asList(items), 1);
    }

    public void add(Iterable<T> items, Integer weight) {
        LinkedList<T> previous = new LinkedList<T>();
        for (T item : items)
        {
            ChainState<T> key = new ChainState<T>((T[])previous.toArray());

            HashMap<T, Integer> weights;
            if (!this.items.containsKey(key))
            {
                weights = new HashMap<T, Integer>();
                this.items.put(key, weights);
            }
            else
            {
                weights = this.items.get(key);
            }

            weights.put(item, weights.containsKey(item)
                ? weight + weights.get(item)
                : weight);

            previous.add(item);
            if (previous.size() > this.order)
            {
                previous.remove();
            }
        }

        ChainState<T> terminalKey = new ChainState<T>((T[])previous.toArray());
        this.terminals.put(terminalKey, this.terminals.containsKey(terminalKey)
            ? weight + this.terminals.get(terminalKey)
            : weight);
    }

    public HashMap<T, Integer> getInitialStates() {
        ChainState<T> startState = new ChainState<T>((T[])new Object[0]);

        if (this.items.containsKey(startState))
        {
            return new HashMap<T, Integer>(this.items.get(startState));
        }

        return null;
    }

    public HashMap<T, Integer> getNextStates(ChainState<T> state) {
        if (this.items.containsKey(state))
        {
            return new HashMap<T, Integer>(this.items.get(state));
        }

        return null;
    }

    public Integer getTerminal(ChainState<T> state) {
        if (this.terminals.containsKey(state))
        {
            return this.terminals.get(state);
        }

        return 0;
    }

    public Iterable<T> chain() {
        return this.chain(new Random());
    }

    public Iterable<T> chain(int seed) {
        return this.chain(new Random(seed));
    }

    public Iterable<T> chain(Random rand) {
        return new ChainIterable(rand, this);
    }

    private class ChainIterable implements Iterable<T> {
        private final Random rand;
        private final MarkovChain<T> generator;

        public ChainIterable(Random rand, MarkovChain<T> generator) {
            this.rand = rand;
            this.generator = generator;
        }

        public Iterator<T> iterator() {
            return new ChainIterator(this.rand, this.generator);
        }
    }

    private class ChainIterator implements Iterator<T> {
        private final Random rand;
        private final MarkovChain<T> generator;

        private final LinkedList<T> previous = new LinkedList<T>();
        
        private boolean hasNext;
        private T next;

        public ChainIterator(Random rand, MarkovChain<T> generator) {
            this.rand = rand;
            this.generator = generator;
            this.hasNext = true;
            
            findNext();
        }

        private void findNext()
        {
            ChainState<T> key = new ChainState<T>((T[])previous.toArray());

            HashMap<T, Integer> weights;
            if (!this.generator.items.containsKey(key))
            {
                this.hasNext = false;
                return;
            }
            else
            {
                weights = this.generator.items.get(key);
            }

            Integer terminalWeight;
            if (this.generator.terminals.containsKey(key))
            {
                terminalWeight = this.generator.terminals.get(key);
            }
            else
            {
                terminalWeight = 0;
            }


            int total = 0;
            for (int w : weights.values())
            {
                total += w;
            }

            Integer value = rand.nextInt(total + terminalWeight) + 1;

            if (value > total)
            {
                this.hasNext = false;
                return;
            }

            int currentWeight = 0;
            for (T itemKey : weights.keySet())
            {
                currentWeight += weights.get(itemKey);
                if (currentWeight >= value)
                {
                    this.next = itemKey;
                    previous.add(itemKey);
                    break;
                }
            }

            if (previous.size() > this.generator.order)
            {
                previous.remove();
            }
        }

        public boolean hasNext() {
            return this.hasNext;
        }

        public T next() {
            T n = this.next;
            this.findNext();
            return n;
        }

        public void remove() {
            throw new UnsupportedOperationException("Removing does not make sense in an iteration.");
        }
    }
}
