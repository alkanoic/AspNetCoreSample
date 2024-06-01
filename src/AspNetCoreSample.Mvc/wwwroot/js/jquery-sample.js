$(document).on("click", ".sample-button", async function () {
  try {
    const response = await fetch(`/jquery/SampleApi`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ text: $(this).siblings(".sample-input").val() }),
    });
    if (response.ok) {
      const r = await response.json();
      $(this)
        .siblings(".sample-label")
        .text(r.text + " " + r.result);
    }
  } catch (error) {
    console.log(error);
  }
});

$(document).on("click", "#loadPartialView", async function () {
  $("#spinner").show();
  $("#loadPartialView").prop("disabled", true);
  try {
    const response = await fetch("/jquery/PartialViewExample", {
      method: "GET",
    });
    if (response.ok) {
      $(this)
        .siblings("#partialViewContainer")
        .html(await response.text());
    }
  } catch (error) {
    console.log(error);
  } finally {
    $("#spinner").hide();
    $("#loadPartialView").prop("disabled", false);
  }
});
