using System;
using System.Text;


namespace Fare
{
    public static class DotFormatter
    {

        public static string ToDot(Automaton a)
        {

            StringBuilder transitions = new StringBuilder(INITIAL_CAPACITY);
            State finalState = null;

            foreach (var state in a.GetStates())
            {
                transitions.Append( BuildDotTransitions(state) );
                finalState = state;
            }

            string header = BuildDotHeader(a.Initial, finalState);

            sb.Clear();
            sb.Append($"{header}\n");
            sb.Append(transitions.ToString());
            sb.Append("\n}");

            return sb.ToString();
        }


        private static string BuildDotHeader(State first, State last)
        {
            StringBuilder b = new StringBuilder(INITIAL_CAPACITY);

            b.Append("digraph finite_state_machine {\n");

            // Designate certain nodes to use double circle?
            if (null != first || null != last)
            {
                b.Append("node [shape = doublecircle]; ");
                if (null != first) { b.Append($" S{first.Id} "); }
                if (null != last) { b.Append($" S{last.Id} "); }
                b.Append("\n");
            }

            // Designate all others a using regular circle.
            //  NOTE: this must be last since nodes use the most recent (last) specification
            b.Append($"node [shape = circle];\n");

            return b.ToString();
        }

        private static string BuildDotTransitions(State s)
        {
            StringBuilder b = new StringBuilder(INITIAL_CAPACITY);

            foreach (var trans in s.GetSortedTransitions(false))
            {
                string label = $"[label = \"{trans.Min}\"]";
                if (trans.Min != trans.Max)
                {
                    label = $"[label = \"[{trans.Min} - {trans.Max}]\"]";
                }

                b.Append($"S{s.Id} -> S{trans.To.Id}  {label} ;\n");
            }

            return b.ToString();
        }


        private const int INITIAL_CAPACITY = 2048; // about 2k
        private static StringBuilder sb = new StringBuilder(INITIAL_CAPACITY);

    }
}

