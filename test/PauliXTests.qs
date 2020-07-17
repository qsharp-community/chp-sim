namespace CHPSimulator.Test {
    open Microsoft.Quantum.Canon;
    open Microsoft.Quantum.Diagnostics;
    open Microsoft.Quantum.Intrinsic;

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation PauliXFlipOneQubit () : Unit {
        using (q = Qubit()) {
            X(q);
            AssertMeasurement([PauliZ],[q], One, "Qubit should have been flipped");
            Reset(q);
        }
        Message("Test passed.");
    }

    @Test("chp.StabilizerSimulator")
    @Test("QuantumSimulator")
    operation PauliXFlipSecondQubit () : Unit {
        using (register = Qubit[2]) {
            X(register[1]);
            AssertMeasurement([PauliZ],[register[0]], Zero, "first qubit shouldn't have been flipped");
            AssertMeasurement([PauliZ],[register[1]], One, "second qubit should have been flipped");
            ResetAll(register);
        }
        Message("Test passed.");
    }
}
