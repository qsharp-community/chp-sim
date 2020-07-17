namespace CHPSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation MeasurementAndMessageTest() : Unit {
        using (q = Qubit()) {
            H(q);
            let r = M(q);
            if (r == Zero) {
                Dog();
            }
            else
            {
                Duck();
            }
            Reset(q);
        }
        Message("Test passed.");
    }

    operation Dog() : Unit 
    { 
        Message("Woof!!"); 
    }

    operation Duck() : Unit 
    { 
        Message("Quack!!"); 
    }
}
