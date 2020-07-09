namespace sample {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Diagnostics;
    

    @EntryPoint()
    operation Test() : Unit {
        using (q = Qubit()) {
            H(q);
            DumpMachine();
            let result = M(q);
            Message($"got {result}");
            DumpMachine();
            H(q);
            DumpMachine();
            H(q);
            DumpMachine();
            X(q);
            if (M(q) == One) { X(q); }
            DumpMachine();
        }
    }
}
