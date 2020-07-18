namespace CHPSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;
    
    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation MeasureZeroTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            let x = M(a);
            let y = M(b);

            EqualityFactR(x,Zero, "Should be |0>");
            EqualityFactR(y,Zero, "Should be |0>");

            AssertMeasurement([PauliZ], [a], Zero, "Should not be changed. Still be |0>");
            AssertMeasurement([PauliZ], [b], Zero, "Should not be changed. Still be |0>");
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation MeasureOneTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateOne(b);
            let x = M(a);
            let y = M(b);

            EqualityFactR(x,Zero, "Measurement should be |0>");
            EqualityFactR(y,One, "Measurement should be |1>");
            AssertMeasurement([PauliZ], [a], Zero, "Should not be changed. Still be |0>");
            AssertMeasurement([PauliZ], [b], One, "Should be |1>");
            Reset(b);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation MeasureOneBothTest() : Unit {
        using ((a,b) = (Qubit(),Qubit())) {
            StateOne(a);
            StateOne(b);
            let x = M(a);
            let y = M(b);

            EqualityFactR(x,One, "Measurement should be |1>");
            EqualityFactR(y,One, "Measurement should be |1>");
            AssertMeasurement([PauliZ], [a], One, "Should be |1>");
            AssertMeasurement([PauliZ], [b], One, "Should be |1>");
            Reset(b);
        }
        Message("Test passed.");
    }
}
