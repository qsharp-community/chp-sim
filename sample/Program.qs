namespace sample {

    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Intrinsic;
    

    @EntryPoint()
    operation Test() : Unit {
        using (q = Qubit()) {
            H(q);
            H(q);
        }
    }
}

