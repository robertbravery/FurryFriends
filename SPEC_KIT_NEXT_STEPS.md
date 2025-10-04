# Spec-Kit Next Steps Guide

## ✅ Current Status: Foundation Complete

You've successfully completed the **foundation setup** for GitHub Spec-Kit:

- ✅ Constitution v2.0.0 created with comprehensive .NET patterns
- ✅ Plan template updated for FurryFriends architecture
- ✅ Documentation structure established
- ✅ Technical Guide linked

## 🎯 What's Next: Two Paths

### Path A: Start Building Features (Recommended)
Jump straight into using Spec-Kit to build a new feature

### Path B: Complete Template Customization
Finish customizing all templates for FurryFriends

---

## 🚀 Path A: Build Your First Feature with Spec-Kit

### The Spec-Kit Feature Workflow

```
User Idea
    ↓
[1] /spec command → specs/###-feature/spec.md
    ↓
[2] /plan command → specs/###-feature/plan.md + research.md + contracts/
    ↓
[3] /tasks command → specs/###-feature/tasks.md
    ↓
[4] Execute tasks → Actual implementation
    ↓
[5] Validate → Tests pass, feature complete
```

### Step 1: Choose a Feature to Build

Pick something small to start with. Examples:
- ✅ **Add booking cancellation** (small, well-defined)
- ✅ **Pet walker availability calendar** (medium complexity)
- ✅ **Client notification preferences** (simple CRUD)
- ❌ **Complete payment system** (too large for first feature)

### Step 2: Run the /spec Command

**Command**: `speckai spec "Add booking cancellation feature"`

**What it does**:
1. Creates `specs/001-booking-cancellation/spec.md`
2. Extracts user scenarios from your description
3. Generates functional requirements
4. Identifies key entities
5. Marks ambiguities with `[NEEDS CLARIFICATION]`

**Output**: A business-focused specification (no tech details)

**Example spec.md**:
```markdown
# Feature Specification: Booking Cancellation

## User Scenarios & Testing
**Primary User Story**: As a client, I want to cancel my booking so that 
I don't get charged for a service I no longer need.

**Acceptance Scenarios**:
1. **Given** a confirmed booking, **When** client cancels 24+ hours before, 
   **Then** full refund issued
2. **Given** a confirmed booking, **When** client cancels <24 hours before, 
   **Then** 50% cancellation fee applied

## Requirements
- **FR-001**: System MUST allow clients to cancel bookings
- **FR-002**: System MUST calculate cancellation fees based on time remaining
- **FR-003**: System MUST [NEEDS CLARIFICATION: refund method not specified]
```

### Step 3: Review and Clarify

1. Open `specs/001-booking-cancellation/spec.md`
2. Look for `[NEEDS CLARIFICATION]` markers
3. Fill in the missing details
4. Remove the markers when resolved

### Step 4: Run the /plan Command

**Command**: `speckai plan specs/001-booking-cancellation/spec.md`

**What it does**:
1. Loads your spec
2. Runs Constitution Check (validates against your v2.0.0 rules)
3. Generates research.md (technical decisions)
4. Creates data-model.md (entities and relationships)
5. Generates API contracts (OpenAPI specs)
6. Creates quickstart.md (manual testing guide)
7. Updates agent context files

**Output**: Complete implementation plan with architecture

**Example plan.md sections**:
```markdown
## Constitution Check
✅ Uses Result pattern for operation outcomes
✅ FluentValidation for CancelBookingCommand
✅ Specification for querying cancellable bookings
✅ Guard clauses in Booking.Cancel() method
✅ Serilog logging for cancellation events

## Phase 1: Design
**Entities Modified**: Booking (add CancellationStatus, CancellationFee)
**New Commands**: CancelBookingCommand
**New Endpoints**: POST /api/bookings/{id}/cancel
**Specifications**: CancellableBookingsSpecification
```

### Step 5: Run the /tasks Command

**Command**: `speckai tasks specs/001-booking-cancellation/plan.md`

**What it does**:
1. Loads plan.md and design docs
2. Generates numbered, ordered tasks
3. Marks parallel tasks with [P]
4. Creates dependency graph
5. Follows TDD order (tests before implementation)

**Output**: `specs/001-booking-cancellation/tasks.md`

**Example tasks.md**:
```markdown
## Phase 3.2: Tests First (TDD)
- [ ] T001 [P] Contract test POST /api/bookings/{id}/cancel
- [ ] T002 [P] Unit test Booking.Cancel() with guard clauses
- [ ] T003 [P] Unit test CancelBookingCommandValidator
- [ ] T004 Integration test cancellation with fee calculation

## Phase 3.3: Core Implementation
- [ ] T005 Add CancellationStatus enum to Booking entity
- [ ] T006 Add Booking.Cancel() method with guard clauses
- [ ] T007 Create CancelBookingCommand + Validator
- [ ] T008 Create CancelBookingHandler returning Result<T>
- [ ] T009 Create CancellableBookingsSpecification
- [ ] T010 Create POST /api/bookings/{id}/cancel endpoint

## Phase 3.4: Integration
- [ ] T011 Add Serilog logging to handler
- [ ] T012 Update Blazor BookingManagement page with Cancel button
- [ ] T013 Add cancellation confirmation popup
```

### Step 6: Execute Tasks

Work through tasks.md in order:
1. ✅ Write tests first (they should fail)
2. ✅ Implement code to make tests pass
3. ✅ Commit after each task
4. ✅ Follow Constitution principles

### Step 7: Validate

1. Run all tests: `dotnet test`
2. Execute quickstart.md manually
3. Verify Constitution compliance
4. Mark feature complete

---

## 🔧 Path B: Complete Template Customization

### Templates Updated So Far

✅ **plan-template.md**:
- Updated to Constitution v2.0.0
- Added FurryFriends-specific Constitution Check
- Added FurryFriends project structure
- Added FurryFriends technology stack

### Templates Still Generic

⚠️ **spec-template.md**: Still generic (works fine as-is)
⚠️ **tasks-template.md**: Still generic (works fine as-is)

### Optional: Customize Remaining Templates

You can customize these if you want FurryFriends-specific examples:

#### spec-template.md Customization Ideas:
- Add FurryFriends domain examples (Client, PetWalker, Booking)
- Add common functional requirements for pet care domain
- Add FurryFriends-specific edge cases

#### tasks-template.md Customization Ideas:
- Update path conventions to FurryFriends structure
- Add FurryFriends-specific task examples
- Reference Constitution v2.0.0 patterns

**Recommendation**: Start with Path A first. Customize templates later based on what you learn.

---

## 📚 Quick Reference

### Spec-Kit Commands

| Command | Purpose | Input | Output |
|---------|---------|-------|--------|
| `speckai spec "description"` | Create feature spec | User description | `specs/###-feature/spec.md` |
| `speckai plan specs/###/spec.md` | Create implementation plan | spec.md | plan.md, research.md, contracts/ |
| `speckai tasks specs/###/plan.md` | Generate task breakdown | plan.md | tasks.md |

### File Structure

```
FurryFriends/
├── .specify/
│   ├── memory/
│   │   └── constitution.md          ← Your governance (v2.0.0)
│   └── templates/
│       ├── spec-template.md         ← Feature spec template
│       ├── plan-template.md         ← Implementation plan template ✅ Updated
│       └── tasks-template.md        ← Task breakdown template
├── specs/
│   └── ###-feature-name/            ← Generated per feature
│       ├── spec.md                  ← Business requirements
│       ├── plan.md                  ← Technical plan
│       ├── research.md              ← Technical decisions
│       ├── data-model.md            ← Entity design
│       ├── contracts/               ← API contracts
│       ├── quickstart.md            ← Manual testing
│       └── tasks.md                 ← Task breakdown
└── src/                             ← Your actual code
```

### Constitution Principles Quick Reference

When implementing features, remember:
- ✅ **Result Pattern**: All handlers return `Result<T>`
- ✅ **FluentValidation**: All commands have validators
- ✅ **Specifications**: All queries use Ardalis.Specification
- ✅ **Guard Clauses**: All methods validate parameters
- ✅ **Serilog**: All operations log structured events
- ✅ **Test-First**: Write tests before implementation

---

## 🎯 Recommended Next Action

### Option 1: Try Spec-Kit with a Small Feature (Recommended)

```bash
# 1. Choose a small feature
# Example: "Add ability for clients to mark pet walkers as favorites"

# 2. Create the spec
speckai spec "Add ability for clients to mark pet walkers as favorites"

# 3. Review and clarify
code specs/001-favorite-petwalkers/spec.md
# Fill in any [NEEDS CLARIFICATION] markers

# 4. Generate the plan
speckai plan specs/001-favorite-petwalkers/spec.md

# 5. Review the plan
code specs/001-favorite-petwalkers/plan.md
# Verify Constitution Check passes

# 6. Generate tasks
speckai tasks specs/001-favorite-petwalkers/plan.md

# 7. Execute tasks
code specs/001-favorite-petwalkers/tasks.md
# Work through tasks in order
```

### Option 2: Commit Your Foundation First

```bash
# Commit all the foundation work
git add .specify/ CONSTITUTION_SUMMARY.md DOCUMENTATION_GUIDE.md SPEC_KIT_NEXT_STEPS.md
git commit -m "feat: complete Spec-Kit foundation with Constitution v2.0.0"
git push

# Then proceed with Option 1
```

---

## 🆘 Troubleshooting

### "speckai command not found"
```bash
npm install -g @speckai/cli
# or
npx @speckai/cli
```

### "Constitution Check fails"
- Review `.specify/memory/constitution.md`
- Ensure your plan follows required patterns
- Document violations in Complexity Tracking section

### "Too many [NEEDS CLARIFICATION] markers"
- This is normal for first spec
- Review each marker and fill in details
- Ask stakeholders if needed
- Remove markers when resolved

---

## 📖 Additional Resources

- **Constitution**: `.specify/memory/constitution.md` - Your governance rules
- **Technical Guide**: `docs/FurryFriends_Technical_Guide.md` - Implementation how-to
- **Documentation Guide**: `DOCUMENTATION_GUIDE.md` - When to use which doc
- **Constitution Summary**: `CONSTITUTION_SUMMARY.md` - Quick reference

---

## ✅ Success Criteria

You'll know Spec-Kit is working when:
1. ✅ You can generate a spec from a feature description
2. ✅ The plan passes Constitution Check automatically
3. ✅ Generated tasks follow FurryFriends patterns
4. ✅ Implementation follows Constitution principles
5. ✅ Tests pass and feature works

---

**Ready to build your first feature with Spec-Kit? Start with Option 1 above! 🚀**

