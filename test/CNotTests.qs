namespace CHPSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation CNotZeroZeroZero() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            CNOT(c,t);
            
            AssertMeasurement([PauliZ],[c], Zero, "Should be untouched");
            AssertMeasurement([PauliZ],[t], Zero, "Should be untouched");
            Reset(c);
            Reset(t);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation CNotZeroOneOne() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            X(t);
            CNOT(c,t);
            
            AssertMeasurement([PauliZ],[c], Zero, "Should be untouched");
            AssertMeasurement([PauliZ],[t], One, "Should be rotated");
            Reset(t);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation CNotOneZeroOne() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            X(c);
            CNOT(c,t);
            
            AssertMeasurement([PauliZ],[c], One, "Should be set");
            AssertMeasurement([PauliZ],[t], One, "Should be rotated");
            Reset(c);
            Reset(t);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation CNotOneOneZero() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            X(c);
            X(t);
            CNOT(c,t);
            AssertMeasurement([PauliZ],[c], One, "Should be untouched");
            AssertMeasurement([PauliZ],[t], One, "Should be rotated");

            Reset(c);
            Reset(t);
        }
        Message("Test passed.");
    }
}
