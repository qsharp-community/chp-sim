namespace CHPSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
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

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
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

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
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

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation CNotOneOneZero() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            X(c);
            X(t);
            CNOT(c,t);
            AssertMeasurement([PauliZ],[c], One, "Should be untouched");
            AssertMeasurement([PauliZ],[t], Zero, "Should be rotated");

            Reset(c);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation InvertedCNotZeroZeroZero() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            H(c);
            H(t);
            CNOT(t,c);
            H(c);
            H(t);
            
            AssertMeasurement([PauliZ],[c], Zero, "Should be untouched");
            AssertMeasurement([PauliZ],[t], Zero, "Should be untouched");
            Reset(c);
            Reset(t);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation InvertedCNotZeroOneOne() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            X(t);
            H(c);
            H(t);
            CNOT(t,c);
            H(c);
            H(t);
            
            AssertMeasurement([PauliZ],[c], Zero, "Should be untouched");
            AssertMeasurement([PauliZ],[t], One, "Should be rotated");
            Reset(t);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation InvertedCNotOneZeroOne() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            X(c);
            H(c);
            H(t);
            CNOT(t,c);
            H(c);
            H(t);
            
            AssertMeasurement([PauliZ],[c], One, "Should be set");
            AssertMeasurement([PauliZ],[t], One, "Should be rotated");
            Reset(c);
            Reset(t);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation InvertedCNotOneOneZero() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            X(c);
            X(t);
            H(c);
            H(t);
            CNOT(t,c);
            H(c);
            H(t);
            AssertMeasurement([PauliZ],[c], One, "Should be untouched");
            AssertMeasurement([PauliZ],[t], Zero, "Should be rotated");

            Reset(c);
        }
        Message("Test passed.");
    }

    @Test("QSharpCommunity.Simulators.Chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation CNotEqual() : Unit {
        using ((c,t) = (Qubit(),Qubit())) {
            H(c);
            CNOT(c,t);

            let left = M(c);
            let right = M(t);
            EqualityFactR(left,right, "Should be the same");

            Reset(c);
            Reset(t);
        }
        Message("Test passed.");
    }
}
