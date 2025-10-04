# FurryFriends Documentation Guide

## Overview

The FurryFriends project has multiple documentation resources, each serving a different purpose. This guide helps you understand when to use each document.

## Documentation Structure

```
FurryFriends/
├── .specify/memory/
│   └── constitution.md          ← Governance & Standards (WHAT & WHY)
├── docs/
│   ├── FurryFriends_Technical_Guide.md  ← Learning & Implementation (HOW)
│   └── #C4 Model High Level Diagram FurryFriend.mmd  ← Architecture Diagrams
├── CONSTITUTION_SUMMARY.md      ← Quick Reference
└── DOCUMENTATION_GUIDE.md       ← This file
```

## Document Purposes

### 1. Constitution (`.specify/memory/constitution.md`)

**Purpose**: Governance, architectural decisions, and non-negotiable standards

**When to Use**:
- ✅ Making architectural decisions
- ✅ Reviewing code for compliance
- ✅ Onboarding new team members (what rules to follow)
- ✅ Resolving debates about patterns or approaches
- ✅ Planning new features (what patterns to use)

**Content Type**:
- Principles (NON-NEGOTIABLE rules)
- Required patterns (Result, FluentValidation, Specifications, etc.)
- Prohibited practices
- Technology stack requirements
- Naming conventions
- File organization standards

**Tone**: Prescriptive
- "MUST", "REQUIRED", "PROHIBITED"
- "All handlers MUST return Result<T>"
- "Guard clauses are NON-NEGOTIABLE"

**Example**:
```
### XI. Result Pattern (Ardalis.Result) (NON-NEGOTIABLE)

All business operations MUST return `Result<T>` or `Result` instead of 
throwing exceptions for expected failures.

**Prohibited**:
- Throwing exceptions for expected business failures
- Returning null to indicate failure
```

---

### 2. Technical Guide (`docs/FurryFriends_Technical_Guide.md`)

**Purpose**: Education, learning, and detailed implementation guidance

**When to Use**:
- ✅ Learning how to implement patterns
- ✅ Understanding why we use certain techniques
- ✅ Getting detailed examples with explanations
- ✅ Onboarding new developers (how to implement)
- ✅ Reference for implementation details

**Content Type**:
- Explanations of concepts (Dependency Injection, CQRS, etc.)
- Step-by-step implementation guides
- Educational examples with context
- "Why" explanations for patterns
- Prerequisites and learning paths

**Tone**: Educational
- "Here's how to...", "For example...", "This allows you to..."
- Explains concepts before showing code
- Provides context and rationale

**Example**:
```
### FluentValidation

FluentValidation is a popular .NET validation library for building 
strongly-typed validation rules. It provides a fluent interface for 
defining validation rules, making it easy to create complex validation logic.

In the FurryFriends solution, FluentValidation is used to validate the 
data that is entered by users. For example...
```

---

### 3. Constitution Summary (`CONSTITUTION_SUMMARY.md`)

**Purpose**: Quick reference and cheat sheet

**When to Use**:
- ✅ Quick lookup during development
- ✅ Pattern decision matrix
- ✅ Finding which principle covers what
- ✅ Quick examples without full context

**Content Type**:
- Condensed principle summaries
- Pattern decision matrix
- Quick code snippets
- Technology stack list

---

### 4. Architecture Diagrams (`docs/*.mmd`)

**Purpose**: Visual representation of system architecture

**When to Use**:
- ✅ Understanding system structure
- ✅ Planning new features
- ✅ Explaining architecture to stakeholders
- ✅ Onboarding (visual overview)

**Content Type**:
- C4 model diagrams
- Sequence diagrams
- Class diagrams
- Component relationships

---

## Comparison Matrix

| Aspect | Constitution | Technical Guide |
|--------|-------------|-----------------|
| **Purpose** | Governance | Education |
| **Audience** | All developers | New/learning developers |
| **Tone** | Prescriptive | Educational |
| **Focus** | WHAT & WHY | HOW |
| **Examples** | Compliance-focused | Learning-focused |
| **Updates** | When standards change | When adding new techniques |
| **Length** | ~1,100 lines | ~245 lines |

## When to Use Which Document

### Scenario: "How do I validate a command?"

**Constitution Answer** (WHAT):
```
XII. FluentValidation (NON-NEGOTIABLE)

All input validation MUST use FluentValidation for commands, queries, 
and API requests.

Required Usage:
- Every Command MUST have a corresponding Validator
- Validators MUST be registered in DI container
```

**Technical Guide Answer** (HOW):
```
FluentValidation is a popular .NET validation library...

Here's an example of how to use FluentValidation:

public class CreatePetWalkerCommandValidator : AbstractValidator<CreatePetWalkerCommand>
{
    public CreatePetWalkerCommandValidator()
    {
        RuleFor(v => v.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required.");
    }
}
```

**Best Approach**: Read Technical Guide to learn HOW, then reference Constitution to ensure compliance with standards.

---

### Scenario: "Should I use Result pattern or throw exceptions?"

**Constitution Answer**:
```
XI. Result Pattern (NON-NEGOTIABLE)

Use Result Pattern for: Expected failures (not found, validation errors)
Use Exceptions for: Unexpected failures (database down, out of memory)

Prohibited:
- Throwing exceptions for expected business failures
```

**Technical Guide Answer**:
```
(Not currently covered in detail - Constitution has more comprehensive guidance)
```

**Best Approach**: Constitution is the authoritative source for this decision.

---

### Scenario: "What is CQRS and how do I implement it?"

**Constitution Answer**:
```
IV. CQRS & MediatR Usage

All business operations MUST use CQRS pattern via MediatR:
- Commands for state changes
- Queries for data retrieval
- Handlers in UseCases project
```

**Technical Guide Answer**:
```
CQRS (Command Query Responsibility Segregation) is a design pattern 
that separates read and write operations...

In the FurryFriends solution, CQRS is implemented using the MediatR library.

Here's an example of how to use MediatR to send a command:
[detailed example with explanation]
```

**Best Approach**: Read Technical Guide for understanding, then follow Constitution for implementation standards.

---

## Keeping Documents in Sync

### When to Update Constitution
- ✅ Changing architectural decisions
- ✅ Adding/removing required patterns
- ✅ Updating technology stack
- ✅ Changing naming conventions
- ✅ Adding new non-negotiable rules

### When to Update Technical Guide
- ✅ Adding new techniques or patterns
- ✅ Improving explanations
- ✅ Adding more examples
- ✅ Covering new technologies
- ✅ Expanding educational content

### Sync Checklist
When updating either document:
1. ✅ Ensure examples follow Constitution principles
2. ✅ Update version numbers and dates
3. ✅ Cross-reference between documents
4. ✅ Update CONSTITUTION_SUMMARY.md if needed
5. ✅ Verify no contradictions between documents

---

## Recommended Reading Order for New Developers

### Day 1: Overview
1. **Constitution Summary** - Get quick overview of principles
2. **Architecture Diagrams** - Understand system structure
3. **Technical Guide - Introduction** - Understand project purpose

### Week 1: Fundamentals
1. **Technical Guide - Intermediate Techniques** - Learn basics
2. **Constitution - Core Principles I-V** - Understand architecture rules
3. **Constitution - Technology Stack** - Know what tools to use

### Week 2: Patterns
1. **Technical Guide - Advanced Techniques** - Learn CQRS, MediatR, etc.
2. **Constitution - Principles XI-XV** - Deep dive into required patterns
3. **Code Examples** - Study existing code with Constitution in mind

### Ongoing: Reference
- **Constitution** - Daily reference for standards compliance
- **Technical Guide** - Reference when implementing new patterns
- **Constitution Summary** - Quick lookups during development

---

## Quick Reference

### "I need to know WHAT to do"
→ **Constitution**

### "I need to know HOW to do it"
→ **Technical Guide**

### "I need a quick example"
→ **Constitution Summary**

### "I need to see the big picture"
→ **Architecture Diagrams**

---

## Document Maintenance

### Constitution Maintenance
- **Owner**: Architecture team
- **Review Frequency**: Quarterly
- **Update Trigger**: Architectural decisions, new patterns
- **Version Control**: Semantic versioning (MAJOR.MINOR.PATCH)

### Technical Guide Maintenance
- **Owner**: Development team
- **Review Frequency**: As needed
- **Update Trigger**: New techniques, improved examples
- **Version Control**: Date-based updates

---

## Conclusion

The Constitution and Technical Guide serve complementary purposes:

- **Constitution** = The LAW (what you MUST follow)
- **Technical Guide** = The TEXTBOOK (how to learn and implement)

Both are essential for a well-functioning development team. Use them together for best results!

