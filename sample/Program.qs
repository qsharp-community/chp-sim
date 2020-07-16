namespace sample {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Diagnostics;
    

    @EntryPoint()
    operation Test() : Unit {
        using (q = Qubit()) {
            DumpMachine();
            let resultZero = M(q);
            EqualityFactR(resultZero, Zero, "X didn't return correct measurement.");            
            // H(q);
            // S(q);
            // S(q);
            // H(q);
            X(q);
            DumpMachine();
            let resultOne = M(q);
            EqualityFactR(resultOne, One, "X didn't return correct measurement.");
            DumpMachine();
//            let result = M(q);
//            Message($"got {result}");
//            DumpMachine();
            if (M(q) == One) { X(q); }
            DumpMachine();
        }
    }
}
