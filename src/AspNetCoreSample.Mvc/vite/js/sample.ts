import { TargetA } from "./interfaces/TargetA";

console.log("aaaa");
console.log("bbbb");

$(document).on("click", $(".target"), function () {
  const a = new TargetA();
  a.Value1 = "a";
  a.Value2 = 123;
  console.log(a);
});
