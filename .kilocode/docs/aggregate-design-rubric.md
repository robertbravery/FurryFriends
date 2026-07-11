\# 🐾 FurryFriends Aggregate Design Rubric

A scoring guide for evaluating aggregate correctness, boundaries, invariants, and constitution compliance.



This rubric ensures that aggregates in the FurryFriends system remain:

\- Cohesive  

\- Invariant-protecting  

\- Constitution-aligned  

\- Behavior-rich  

\- Persistence-ignorant  

\- CQRS-correct  

\- Feature-aligned (SpecKit)  



Use this rubric during:

\- Domain modeling  

\- Code reviews  

\- Multi-agent reviews  

\- Feature development  

\- Architecture enforcement  



\---



\## 🧩 1. Aggregate Identity \& Purpose (0–10)



\### Questions

\- Does the aggregate represent \*\*one cohesive business concept\*\*?

\- Does it have \*\*one clear aggregate root\*\*?

\- Is the root the \*\*only entry point\*\* for mutations?

\- Does the name match the \*\*ubiquitous language\*\*?



\### Scoring

\- \*\*0–3\*\*: unclear purpose, multiple concepts mixed  

\- \*\*4–7\*\*: mostly cohesive, minor leakage  

\- \*\*8–10\*\*: perfectly scoped, clear root, domain-aligned  



\---



\## 🧩 2. Invariant Enforcement (0–10)



\### Questions

\- Are invariants enforced \*\*immediately\*\* when state changes?

\- Are invariants enforced \*\*inside the aggregate\*\*, not externally?

\- Are invariants \*\*explicit\*\*, not implicit?

\- Are invariants \*\*business rules\*\*, not technical rules?



\### Scoring

\- \*\*0–3\*\*: invariants missing or enforced outside  

\- \*\*4–7\*\*: some invariants enforced, some missing  

\- \*\*8–10\*\*: all invariants enforced correctly inside the aggregate  



\---



\## 🧩 3. Behavior-Richness (0–10)



\### Questions

\- Does the aggregate expose \*\*behavior\*\*, not just getters/setters?

\- Are methods named after \*\*domain actions\*\*?

\- Does the aggregate avoid being a \*\*data container\*\*?



\### Scoring

\- \*\*0–3\*\*: anemic domain model  

\- \*\*4–7\*\*: mixed behavior/data  

\- \*\*8–10\*\*: fully behavior-rich domain model  



\---



\## 🧩 4. Child Entities \& Collections (0–10)



\### Questions

\- Are child entities \*\*never mutated directly\*\*?

\- Are collections encapsulated?

\- Are operations on children done through \*\*root methods\*\*?



\### Scoring

\- \*\*0–3\*\*: direct mutation of children  

\- \*\*4–7\*\*: mostly correct, minor leaks  

\- \*\*8–10\*\*: perfect encapsulation  



\---



\## 🧩 5. Value Object Usage (0–10)



\### Questions

\- Are domain concepts modeled as \*\*value objects\*\*?

\- Are value objects \*\*self-validating\*\*?

\- Are primitives avoided?



\### Scoring

\- \*\*0–3\*\*: primitive obsession  

\- \*\*4–7\*\*: mixed usage  

\- \*\*8–10\*\*: excellent value object usage  



\---



\## 🧩 6. Domain Events (0–10)



\### Questions

\- Are domain events raised for \*\*meaningful business occurrences\*\*?

\- Are events raised \*\*inside the aggregate\*\*?

\- Are events \*\*pure domain events\*\*, not DTOs?



\### Scoring

\- \*\*0–3\*\*: no events or incorrect usage  

\- \*\*4–7\*\*: some events, inconsistent  

\- \*\*8–10\*\*: excellent event modeling  



\---



\## 🧩 7. Aggregate Size \& Cohesion (0–10)



\### Questions

\- Is the aggregate \*\*small\*\*?

\- Does it avoid becoming a “god object”?

\- Does every part of the aggregate support the \*\*same invariant\*\*?



\### Scoring

\- \*\*0–3\*\*: too large, multiple responsibilities  

\- \*\*4–7\*\*: mostly cohesive  

\- \*\*8–10\*\*: small, tight, cohesive  



\---



\## 🧩 8. Persistence Ignorance (0–10)



\### Questions

\- Does the aggregate avoid EF Core attributes?

\- Does it avoid infrastructure concerns?

\- Does it avoid persistence logic?



\### Scoring

\- \*\*0–3\*\*: EF Core leakage  

\- \*\*4–7\*\*: minor leakage  

\- \*\*8–10\*\*: fully persistence-ignorant  



\---



\## 🧩 9. CQRS Alignment (0–10)



\### Questions

\- Are commands the \*\*only\*\* way to mutate aggregates?

\- Are queries \*\*never\*\* returning aggregates?

\- Are aggregates \*\*never\*\* used in read models?



\### Scoring

\- \*\*0–3\*\*: CQRS violations  

\- \*\*4–7\*\*: mostly correct  

\- \*\*8–10\*\*: perfect CQRS alignment  



\---



\## 🧩 10. Constitution Compliance (0–10)



\### Questions

\- Does the aggregate follow \*\*all constitution rules\*\*?

\- Are boundaries respected?

\- Are responsibilities correctly placed?



\### Scoring

\- \*\*0–3\*\*: major constitution violations  

\- \*\*4–7\*\*: minor issues  

\- \*\*8–10\*\*: fully compliant  



\---



\## 🧩 11. Feature Folder Alignment (0–10)



\### Questions

\- Does the aggregate match the active feature’s:

 - `spec.md`

 - `plan.md`

 - `tasks.md`

\- Does the aggregate implement the \*\*domain responsibilities\*\* described in the feature?



\### Scoring

\- \*\*0–3\*\*: misaligned with feature  

\- \*\*4–7\*\*: partial alignment  

\- \*\*8–10\*\*: perfect alignment  



\---



\## 🧩 12. Overall Aggregate Score



Add all categories:





