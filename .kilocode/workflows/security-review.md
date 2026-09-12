# Security Review

## Purpose

## Evaluate the codebase, architecture, and specifications for security vulnerabilities, insecure patterns, and risks that may impact confidentiality, integrity, or availability. This review ensures alignment with the Constitution, engineering best practices, and secure‑by‑default principles.

## Review Scope

### Code-Level Security

- Input validation and sanitization

- Injection vulnerabilities (SQL, command, template, expression)

- Authentication and authorization correctness

- Session and token handling

- Cryptographic usage (hashing, encryption, key management)

- Error handling and information leakage

- File system and network access safety

- Dependency and package security

- Logging and audit trails

### Architecture-Level Security

- Trust boundaries

- Data flow and exposure points

- External integrations and API surfaces

- Secrets management

- Configuration safety

- Deployment and environment isolation

- Least privilege enforcement

- Secure defaults

### Domain-Level Security

- Invariant protection

- Aggregate boundary enforcement

- Unauthorized state transitions

- Business rule integrity

- Abuse case handling

### Specification-Level Security

- Requirements completeness for security controls

- Misalignment with Constitution principles

- Missing security acceptance criteria

- Risk classification and mitigation completeness

---

## Rubric

### 1. **Critical Vulnerabilities**

Issues that allow:

- Unauthorized access

- Privilege escalation

- Data corruption or leakage

- Remote code execution

- Broken authentication or authorization

**Score:** 0–5

**Goal:** No critical vulnerabilities.

---

### 2. **High-Risk Weaknesses**

Weaknesses that significantly reduce security posture:

- Weak cryptography

- Unsafe defaults

- Missing validation

- Insecure configuration

- Unsafe dependency usage

**Score:** 0–5

**Goal:** Minimal high-risk weaknesses.

---

### 3. **Moderate Issues**

Problems that could lead to future vulnerabilities:

- Poor error handling

- Missing logging

- Weak boundary enforcement

- Ambiguous requirements

**Score:** 0–5

**Goal:** Addressable with normal development effort.

---

### 4. **Low-Risk Findings**

Minor issues:

- Style inconsistencies

- Non-critical missing checks

- Minor documentation gaps

**Score:** 0–5

**Goal:** Improve over time.

---

### 5. **Security Alignment with Constitution**

Evaluate alignment with:

- Safety

- Integrity

- Transparency

- Reliability

- Privacy

- Resilience

**Score:** 0–5

**Goal:** Strong alignment.

---

## Output Format

The reviewer must produce:

### **Summary**

High-level overview of the security posture.

### **Findings**

List each finding with:

- Title

- Severity (Critical, High, Moderate, Low)

- Description

- Impact

- Evidence

- Recommendation

### **Scores**

Provide rubric scores:

- Critical Vulnerabilities: X/5

- High-Risk Weaknesses: X/5

- Moderate Issues: X/5

- Low-Risk Findings: X/5

- Constitution Alignment: X/5

### **Final Assessment**

## A concise statement of overall security readiness.

## Reviewer Instructions

You are a security auditor. Please follow these steps:

- Apply strict, industry-grade security review standards.
- Be thorough, objective, and evidence-driven.
- Reference Constitution principles where relevant.
- Highlight both strengths and weaknesses.
- Provide actionable recommendations.

---

## Prompt
