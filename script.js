// JavaScript code will go here
const testimonials = document.querySelector('.testimonial-container');
let currentTestimonial = 0;

function showTestimonial() {
    const testimonialWidth = document.querySelector('.testimonial').offsetWidth;
    testimonials.style.transform = `translateX(-${currentTestimonial * testimonialWidth}px)`;
}

function nextTestimonial() {
    currentTestimonial++;
    if (currentTestimonial >= testimonials.children.length) {
        currentTestimonial = 0;
    }
    showTestimonial();
}

setInterval(nextTestimonial, 5000); // Change testimonial every 5 seconds
